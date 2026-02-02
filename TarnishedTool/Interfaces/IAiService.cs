// 

using System;
using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IAiService
{
    nint GetTopGoal(nint aiThink);
    GoalIns GetGoalInfo(nint goalPtr);
    bool HasSubGoals(nint topGoal);
    List<nint> GetSubGoals(nint goalPtr);
    float[] GetLuaTimers(nint aiThink);
    float[] GetLuaNumbers(nint aiThink);
    List<SpEffectObserve> GetSpEffectObserveList(nint aiThink);
    nint GetAiThinkPtr(nint chrIns);
    void RegisterInterruptListener(Action callBack);
    void UnregisterInterruptListener(Action callBack);
    ulong GetInterrupts(nint aiThink);
    List<CoolTimeEntry> GetCoolTimeItemList(nint aiThink);
}