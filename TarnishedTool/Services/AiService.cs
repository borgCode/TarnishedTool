// 

using System;
using System.Collections.Generic;
using System.Numerics;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class AiService(MemoryService memoryService) : IAiService
{
    public const int ChrInsEntrySize = 0x8;

    #region Public Methods

    public List<ChrInsEntry> GetNearbyChrInsEntries()
    {
        var worldChrMan = memoryService.Read<IntPtr>(WorldChrMan.Base);
        nint begin = memoryService.Read<nint>(worldChrMan + WorldChrMan.ChrInsByUpdatePrioBegin);
        nint end = memoryService.Read<nint>(worldChrMan + WorldChrMan.ChrInsByUpdatePrioEnd);

        var count = (end - begin) / ChrInsEntrySize;

        byte[] buffer = memoryService.ReadBytes(begin, (int)(count * ChrInsEntrySize));
        var entries = new List<ChrInsEntry>();
        for (int i = 0; i < count; i++)
        {
            var entry = ParseEntry(buffer, i * ChrInsEntrySize);
            entries.Add(entry);
        }

        return entries;
    }

    public int GetChrIdByChrIns(IntPtr chrIns) =>
        memoryService.Read<int>(chrIns + ChrIns.ChrId);

    public uint GetNpcParamIdByChrIns(IntPtr chrIns) =>
        memoryService.Read<uint>(chrIns + ChrIns.NpcParamId);

    public int GetNpcThinkParamIdByChrIns(IntPtr chrIns) =>
        memoryService.Read<int>(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.NpcThinkParamId);

    public long GetHandleByChrIns(IntPtr chrIns) =>
        memoryService.Read<long>(chrIns + ChrIns.Handle);

    public void SetSelected(nint chrIns, bool isSelected) =>
        memoryService.SetBitValue(GetChrInsFlagsPtr(chrIns), (int)ChrIns.ChrInsFlags.SelectedEntity, isSelected);

    public Position GetChrInsPos(IntPtr chrIns)
    {
        var blockId = memoryService.Read<uint>(chrIns + ChrIns.BlockId);

        var worldBlockInfo = FindWorldBlockInfoByBlockId(blockId);

        Vector3 blockInfoPos = memoryService.Read<Vector3>(worldBlockInfo + 0x70);
        Vector3 chrInsLocalPos = GetChrInsLocalPos(chrIns);

        return new Position(blockId, Vector3.Subtract(chrInsLocalPos, blockInfoPos), 0);
    }

    public Vector3 GetChrInsLocalPos(IntPtr chrIns) =>
        memoryService.Read<Vector3>(GetChrPhysicsPtr(chrIns) + (int)ChrIns.ChrPhysicsOffsets.Coords);

    public nint GetTopGoal(nint chrIns) =>
        memoryService.Read<nint>(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.TopGoal);

    public GoalIns GetGoalInfo(nint goalPtr)
    {
        var goalId = memoryService.Read<int>(goalPtr + ChrIns.AiThinkOffsets.Goal.GoalId);
        var life = memoryService.Read<float>(goalPtr + ChrIns.AiThinkOffsets.Goal.GoalLife);
        var turnTime = memoryService.Read<float>(goalPtr + ChrIns.AiThinkOffsets.Goal.TurnTime);
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
        memoryService.Read<int>(topGoal + ChrIns.AiThinkOffsets.Goal.SubGoalCount) > 0;

    public List<nint> GetSubGoals(nint goalPtr)
    {
        List<nint> childGoals = new List<nint>();

        var subGoalContainer = goalPtr + ChrIns.AiThinkOffsets.Goal.SubGoalContainer;
        var startIdx =
            memoryService.Read<ulong>(subGoalContainer + ChrIns.AiThinkOffsets.SubGoalContainerOffsets.StartIdx);
        var count =
            memoryService.Read<ulong>(subGoalContainer + ChrIns.AiThinkOffsets.SubGoalContainerOffsets.Count);

        var dequeHandle = memoryService.FollowPointers(
            subGoalContainer + ChrIns.AiThinkOffsets.SubGoalContainerOffsets.DequeHandle,
            [0, 0, 0], true);
        var blockMap = memoryService.Read<nint>(dequeHandle + ChrIns.AiThinkOffsets.DequeInternalOffsets.BlockMap);
        var mapCapacity =
            memoryService.Read<ulong>(dequeHandle + ChrIns.AiThinkOffsets.DequeInternalOffsets.MapCapacity);

        if (blockMap == IntPtr.Zero || mapCapacity == 0) return childGoals;

        var endIdx = startIdx + count;

        for (ulong i = startIdx; i < endIdx; i++)
        {
            var blockIdx = (i >> 1) & (mapCapacity - 1);
            var slotIndex = (int)(i & 1);

            var block = memoryService.Read<nint>(blockMap + (nint)(blockIdx * 8));
            if (block == IntPtr.Zero) continue;

            var childGoal = memoryService.Read<nint>(block + slotIndex * 8);
            if (childGoal == IntPtr.Zero) continue;
            childGoals.Add(childGoal);
        }

        return childGoals;
    }

    #endregion

    #region Private Methods

    private ChrInsEntry ParseEntry(byte[] buffer, int offset)
    {
        IntPtr chrIns = (IntPtr)BitConverter.ToInt64(buffer, offset);

        return new ChrInsEntry(chrIns);
    }

    private nint GetAiThinkPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.AiThink], true, false);

    private IntPtr GetChrInsFlagsPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [ChrIns.Flags], false, false);

    private IntPtr GetChrPhysicsPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.ChrPhysicsModule], true, false);

    private IntPtr FindWorldBlockInfoByBlockId(uint blockId)
    {
        var worldInfoOwner =
            memoryService.Read<IntPtr>(memoryService.Read<IntPtr>(FieldArea.Base) + FieldArea.WorldInfoOwner);

        var areaCount = memoryService.Read<int>(worldInfoOwner + FieldArea.WorldInfoOwnerOffsets.AreaCount);
        var areaArrayBase = worldInfoOwner + FieldArea.WorldInfoOwnerOffsets.AreaArrayBase;

        var targetArea = (blockId >>> 24) & 0xFF;

        for (int i = 0; i < areaCount; i++)
        {
            var areaPtr = memoryService.Read<nint>(areaArrayBase + i * 8);

            var areaId = memoryService.Read<int>(areaPtr + 0xC);

            if (areaId == targetArea)
            {
                var blockCount = memoryService.Read<int>(areaPtr + 0x40);
                var blocksPtr = memoryService.Read<nint>(areaPtr + 0x48);

                for (int j = 0; j < blockCount; j++)
                {
                    var blockInfoPtr = blocksPtr + j * 0xE0;
                    var storedBlockId = memoryService.Read<uint>(blockInfoPtr + 0x8);

                    if (storedBlockId == blockId)
                    {
                        return blockInfoPtr;
                    }
                }

                break;
            }
        }

        return IntPtr.Zero;
    }

    private float[] ReadGoalParams(nint goalPtr)
    {
        var baseParams = memoryService.ReadArray<float>(goalPtr + ChrIns.AiThinkOffsets.Goal.InlineParams, 8);
        var extraBegin = memoryService.Read<nint>(goalPtr + ChrIns.AiThinkOffsets.Goal.ExtraParamsBegin);
        var extraEnd = memoryService.Read<nint>(goalPtr + ChrIns.AiThinkOffsets.Goal.ExtraParamsEnd);

        if (extraBegin == 0) return baseParams;

        var extraCount = (int)(extraEnd - extraBegin) / 4;

        if (extraCount <= 0) return baseParams;

        var extraParams = memoryService.ReadArray<float>(extraBegin, extraCount);

        var result = new float[8 + extraCount];
        baseParams.CopyTo(result, 0);
        extraParams.CopyTo(result, 8);

        return result;
    }

    #endregion
}