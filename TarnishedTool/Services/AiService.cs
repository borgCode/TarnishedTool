// 

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class AiService : IAiService
{
    public const int NumOfLuaTimers = 16;
    public const int NumOfLuaNumbers = 64;
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(8) };
    private readonly IMemoryService _memoryService;
    private readonly List<Action> _subscribers = new();

    public const int CoolTimeListStride = 0x14;

    public AiService(IMemoryService memoryService)
    {
        _memoryService = memoryService;
        _timer.Tick += InterruptTick;
    }

    #region Public Methods

    public nint GetTopGoal(nint aiThink) =>
        _memoryService.Read<nint>(aiThink + ChrIns.AiThinkOffsets.TopGoal);

    public GoalIns GetGoalInfo(nint goalPtr)
    {
        var goalId = _memoryService.Read<int>(goalPtr + ChrIns.AiThinkOffsets.Goal.GoalId);
        var life = _memoryService.Read<float>(goalPtr + ChrIns.AiThinkOffsets.Goal.GoalLife);
        var turnTime = _memoryService.Read<float>(goalPtr + ChrIns.AiThinkOffsets.Goal.TurnTime);
        var goalParams = ReadGoalParams(goalPtr);

        return new GoalIns
        {
            GoalId = goalId,
            Life = life,
            TurnTime = turnTime,
            Params = goalParams
        };
    }

    public bool HasSubGoals(nint topGoal) =>
        _memoryService.Read<int>(topGoal + ChrIns.AiThinkOffsets.Goal.SubGoalCount) > 0;

    public List<nint> GetSubGoals(nint goalPtr)
    {
        List<nint> childGoals = new List<nint>();

        var subGoalContainer = goalPtr + ChrIns.AiThinkOffsets.Goal.SubGoalContainer;
        var startIdx =
            _memoryService.Read<ulong>(subGoalContainer + ChrIns.AiThinkOffsets.SubGoalContainerOffsets.StartIdx);
        var count =
            _memoryService.Read<ulong>(subGoalContainer + ChrIns.AiThinkOffsets.SubGoalContainerOffsets.Count);

        var dequeHandle = _memoryService.FollowPointers(
            subGoalContainer + ChrIns.AiThinkOffsets.SubGoalContainerOffsets.DequeHandle,
            [0, 0, 0], true);
        var blockMap = _memoryService.Read<nint>(dequeHandle + ChrIns.AiThinkOffsets.DequeInternalOffsets.BlockMap);
        var mapCapacity =
            _memoryService.Read<ulong>(dequeHandle + ChrIns.AiThinkOffsets.DequeInternalOffsets.MapCapacity);

        if (blockMap == IntPtr.Zero || mapCapacity == 0) return childGoals;

        var endIdx = startIdx + count;

        for (ulong i = startIdx; i < endIdx; i++)
        {
            var blockIdx = (i >> 1) & (mapCapacity - 1);
            var slotIndex = (int)(i & 1);

            var block = _memoryService.Read<nint>(blockMap + (nint)(blockIdx * 8));
            if (block == IntPtr.Zero) continue;

            var childGoal = _memoryService.Read<nint>(block + slotIndex * 8);
            if (childGoal == IntPtr.Zero) continue;
            childGoals.Add(childGoal);
        }

        return childGoals;
    }

    public float[] GetLuaTimers(nint aiThink) =>
        _memoryService.ReadArray<float>(aiThink + ChrIns.AiThinkOffsets.LuaTimersArray, NumOfLuaTimers);

    public float[] GetLuaNumbers(nint aiThink) =>
        _memoryService.ReadArray<float>(aiThink + ChrIns.AiThinkOffsets.LuaNumbersArray, NumOfLuaNumbers);

    public List<SpEffectObserve> GetSpEffectObserveList(nint aiThink)
    {
        List<SpEffectObserve> spEffectObserveList = [];
        var spEffectObserveComponent = aiThink + ChrIns.AiThinkOffsets.SpEffectObserveComp;
        var head = _memoryService.Read<nint>(spEffectObserveComponent + ChrIns.AiThinkOffsets.SpEffectObserve.Head);
        var next = _memoryService.Read<nint>(head + ChrIns.AiThinkOffsets.SpEffectObserveEntry.Next);

        while (next != head && next != 0)
        {
            var target = _memoryService.Read<int>(next + ChrIns.AiThinkOffsets.SpEffectObserveEntry.Target);
            var spEffectId = _memoryService.Read<int>(next + ChrIns.AiThinkOffsets.SpEffectObserveEntry.SpEffectId);

            spEffectObserveList.Add(new SpEffectObserve(target, spEffectId));

            next = _memoryService.Read<nint>(next);
        }

        return spEffectObserveList;
    }

    public nint GetAiThinkPtr(nint chrIns) =>
        _memoryService.FollowPointers(chrIns, [..ChrIns.AiThink], true, false);

    public void RegisterInterruptListener(Action callBack)
    {
        if (_subscribers.Contains(callBack)) return;
        _subscribers.Add(callBack);
        if (_subscribers.Count == 1) _timer.Start();
    }

    public void UnregisterInterruptListener(Action callBack)
    {
        _subscribers.Remove(callBack);
        if (_subscribers.Count == 0) _timer.Stop();
    }

    public ulong GetInterrupts(nint aiThink) =>
        _memoryService.Read<ulong>(aiThink + ChrIns.AiThinkOffsets.Interrupts);

    public List<CoolTimeEntry> GetCoolTimeItemList(nint aiThink)
    {
        var attackComp = aiThink + ChrIns.AiThinkOffsets.AiAttackComp;

        var coolTimeCount = _memoryService.Read<int>(attackComp + ChrIns.AiThinkOffsets.AttackComp.CoolTimeCount);
        if (coolTimeCount == 0) return new List<CoolTimeEntry>();

        var listStart = attackComp + ChrIns.AiThinkOffsets.AttackComp.CoolTimeList;
        var block = new MemoryBlock(_memoryService.ReadBytes(listStart, coolTimeCount * CoolTimeListStride));
        
        var coolTimeList = new List<CoolTimeEntry>(coolTimeCount);
        for (var i = 0; i < coolTimeCount; i++)
        {
            var offset = i * CoolTimeListStride;
            coolTimeList.Add(new CoolTimeEntry(
                block.Get<int>(offset),
                block.Get<float>(offset + ChrIns.AiThinkOffsets.CoolTimeItem.TimeSinceLastAttack),
                block.Get<float>(offset + ChrIns.AiThinkOffsets.CoolTimeItem.Cooldown)
            ));
        }

        return coolTimeList;
    }

    public int GetMainScriptGoalId(nint aiThink)
    {
        var topGoalChildGoalIdPtr = _memoryService.FollowPointers(
            aiThink + ChrIns.AiThinkOffsets.TopGoal,
            [ChrIns.AiThinkOffsets.Goal.ChildGoal, ChrIns.AiThinkOffsets.Goal.GoalId], false);
        return _memoryService.Read<int>(topGoalChildGoalIdPtr);
    }

    public void InjectAiScript(byte[] script)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.LuaDoString);
        var luaState = _memoryService.FollowPointers(WorldAiManagerImp.Base, WorldAiManagerImp.LuaState, true);

        var scriptPtr = _memoryService.AllocateMem((uint)script.Length);
        _memoryService.WriteBytes(scriptPtr, script);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (luaState, 0x0 + 2),
            (scriptPtr, 0xA + 2),
            (Functions.LuaDoString, 0x14 + 2)
        ]);
        _memoryService.AllocateAndExecute(bytes);
        _memoryService.FreeMem(scriptPtr);
    }

    #endregion

    #region Private Methods

    private float[] ReadGoalParams(nint goalPtr)
    {
        var baseParams = _memoryService.ReadArray<float>(goalPtr + ChrIns.AiThinkOffsets.Goal.InlineParams, 8);
        var extraBegin = _memoryService.Read<nint>(goalPtr + ChrIns.AiThinkOffsets.Goal.ExtraParamsBegin);
        var extraEnd = _memoryService.Read<nint>(goalPtr + ChrIns.AiThinkOffsets.Goal.ExtraParamsEnd);

        if (extraBegin == 0) return baseParams;

        var extraCount = (int)(extraEnd - extraBegin) / 4;

        if (extraCount <= 0) return baseParams;

        var extraParams = _memoryService.ReadArray<float>(extraBegin, extraCount);

        var result = new float[8 + extraCount];
        baseParams.CopyTo(result, 0);
        extraParams.CopyTo(result, 8);

        return result;
    }

    private void InterruptTick(object sender, EventArgs e)
    {
        foreach (var subscriber in _subscribers)
            subscriber();
    }

    #endregion
}