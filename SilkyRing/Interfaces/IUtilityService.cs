// 

namespace SilkyRing.Interfaces;

public interface IUtilityService
{
    void ForceSave();
    void ToggleCombatMap(bool isEnabled);
    void ToggleDungeonWarp(bool isEnabled);
    void ToggleNoClip(bool isEnabled);
    void WriteNoClipSpeed(float speedMultiplier);
    float GetSpeed();
    void SetSpeed(float speed);
    void ToggleFreeCam(bool isEnabled);
    void ToggleFreezeWorld(bool isEnabled);
    void ToggleDrawHitbox(bool isEnabled);

}