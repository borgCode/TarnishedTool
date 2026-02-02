// 

using System;
using System.Collections.Generic;
using System.Numerics;
using TarnishedTool.Models;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Interfaces;

public interface IChrInsService
{
    List<ChrInsEntry> GetNearbyChrInsEntries();
    int GetChrId(IntPtr chrIns);
    uint GetNpcParamId(IntPtr chrIns);
    long GetHandleByChrIns(IntPtr chrIns);
    void SetSelected(nint chrIns, bool isSelected);
    Position GetChrInsMapCoords(IntPtr chrIns);
    Vector3 GetLocalCoords(IntPtr chrIns);
    void ToggleTargetAi(IntPtr chrIns, bool isDisableTargetAiEnabled);
    bool IsAiDisabled(IntPtr chrIns);
    void ToggleTargetView(IntPtr chrIns, bool isTargetViewEnabled);
    bool IsTargetViewEnabled(IntPtr chrIns);
    void ToggleNoAttack(nint chrIns, bool isEnabled);
    bool IsNoAttackEnabled(IntPtr chrIns);
    void ToggleNoMove(nint chrIns, bool isEnabled);
    bool IsNoMoveEnabled(IntPtr chrIns);
    void ToggleNoDamage(nint chrIns, bool isEnabled);
    bool IsNoDamageEnabled(IntPtr chrIns);
    void SetHp(nint chrIns, int health);
    int GetCurrentHp(nint chrIns);
    int GetMaxHp(nint chrIns);
    float GetCurrentPoise(nint chrIns);
    float GetMaxPoise(nint chrIns);
    float GetPoiseTimer(nint chrIns);
    float GetSpeed(nint chrIns);
    void SetSpeed(nint chrIns, float speed);
    float[] GetDefenses(nint chrIns);
    bool[] GetImmunities(nint chrIns);
    int GetResistance(nint chrIns, int offset);
    uint GetEntityId(nint chrIns);
    int GetNpcThinkParamId(nint chrIns);
    float GetDistBetweenChrs(nint chrIns1, nint chrIns2);
    int GetCurrentAnimation(nint chrIns);
    void SetLocalCoords(nint chrIns, Vector3 pos);
    bool IsNoDeathEnabled(nint chrIns);
    nint ChrInsByHandle(int handle);
    nint ChrInsByEntityId(uint entityId);
    void ToggleNoDeath(nint chrIns, bool isEnabled);
}