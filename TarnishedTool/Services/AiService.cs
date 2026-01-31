// 

using System;
using System.Collections.Generic;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services;

public class AiService(MemoryService memoryService) : IAiService
{
    #region Public Methods

    public int GetNpcThinkParamIdByChrIns(IntPtr chrIns) =>
        memoryService.Read<int>(GetAiThinkPtr(chrIns) + ChrIns.AiThinkOffsets.NpcThinkParamId);

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

    private nint GetAiThinkPtr(IntPtr chrIns) =>
        memoryService.FollowPointers(chrIns, [..ChrIns.AiThink], true, false);

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