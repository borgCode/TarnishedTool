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
    int GetChrIdByChrIns(IntPtr chrIns);
    uint GetNpcParamIdByChrIns(IntPtr chrIns);
    long GetHandleByChrIns(IntPtr chrIns);
    void SetSelected(nint chrIns, bool isSelected);
    Position GetChrInsMapCoords(IntPtr chrIns);
    Vector3 GetChrInsLocalPos(IntPtr chrIns);
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
    
    
}