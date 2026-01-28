// 

using System;

namespace TarnishedTool.Interfaces;

public interface ITargetService
{
    void ToggleTargetHook(bool isEnabled);
    long GetTargetChrIns();
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();
    float GetCurrentPoise();
    float GetMaxPoise();
    float GetPoiseTimer();
    float[] GetPosition();
    float GetSpeed();
    void SetSpeed(float speed);
    void ToggleTargetAi(bool isDisableTargetAiEnabled);
    bool IsAiDisabled();
    void ToggleNoAttack(bool isNoAttackEnabled);
    bool IsNoAttackEnabled();
    void ToggleNoMove(bool isNoMoveEnabled);
    bool IsNoMoveEnabled(); 
    void ForceAct(int forceAct);
    int GetLastAct();
    int GetForceAct();
    void ToggleRepeatAct(bool isRepeatActEnabled);
    bool IsTargetRepeating();
    int GetCurrentAnimation();
    void SetAnimation(int animationId);
    void ToggleTargetingView(bool isTargetingViewEnabled);
    bool IsTargetViewEnabled();
    void ToggleTargetNoDamage(bool isFreezeHealthEnabled);
    bool IsNoDamageEnabled();
    void ToggleNoStagger(bool isEnabled);
    void KillAllBesidesTarget();
    void ToggleDisableAllExceptTarget(bool isEnabled);
    int GetNpcThinkParamId();
    int GetResistance(int offset);
    bool[] GetImmunities();
    float[] GetDefenses();
    float GetDist();
    uint GetEntityId();
    int GetNpcChrId();
    uint GetNpcParamId();
    void ToggleNoHeal(bool isNoHealEnabled);
    IntPtr GetAiThinkPtr();
}