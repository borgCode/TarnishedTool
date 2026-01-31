// 

using System;
using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IAiService
{
    int GetNpcThinkParamIdByChrIns(IntPtr chrIns);
    nint GetTopGoal(nint chrIns);
    GoalIns GetGoalInfo(nint goalPtr);
    bool HasSubGoals(nint topGoal);
    List<nint> GetSubGoals(nint goalPtr);
}