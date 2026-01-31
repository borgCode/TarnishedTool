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
    float[] GetLuaTimers(nint chrIns);
    float[] GetLuaNumbers(nint chrIns);
    List<SpEffectObserve> GetSpEffectObserveList(nint chrIns);
    nint GetAiThinkPtr(nint chrIns);
    void RegisterInterruptListener(Action callBack);
    void UnregisterInterruptListener(Action callBack);
    ulong GetInterrupts(nint aiThink);
}