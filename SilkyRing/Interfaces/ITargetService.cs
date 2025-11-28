// 

namespace SilkyRing.Interfaces;

public interface ITargetService
{
    void ToggleTargetHook(bool isEnabled);
    ulong GetTargetAddr();
    
    void SetHp(int hp);
    int GetCurrentHp();
    int GetMaxHp();
    
    // float GetCurrentPoise();
    // float GetMaxPoise();
    // float GetPoiseTimer();
    
    float[] GetPosition();

    void ClearLockedTarget();
    void ToggleTargetAi(bool isDisableTargetAiEnabled);
    bool IsAiDisabled();
    void ForceAct(int forceAct);
    int GetLastAct();
    int GetForceAct();
    void ToggleRepeatAct(bool isRepeatActEnabled);
    bool IsTargetRepeating();
    void ToggleTargetingView(bool isTargetingViewEnabled);
    bool IsTargetViewEnabled();
    void ToggleTargetNoDamage(bool isFreezeHealthEnabled);
    bool IsTargetNoDamageEnabled();
}