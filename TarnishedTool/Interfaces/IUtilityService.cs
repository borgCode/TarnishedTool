// 

namespace TarnishedTool.Interfaces;

public interface IUtilityService
{
    void ForceSave();
    void TriggerNewNgCycle();
    void ToggleCombatMap(bool isEnabled);
    void ToggleDungeonWarp(bool isEnabled);
    void ToggleNoClip(bool isEnabled);
    void WriteNoClipSpeed(float speedMultiplier);
    float GetSpeed();
    void SetSpeed(float speed);
    void ToggleFreeCam(bool isEnabled);
    void MoveCamToPlayer();
    void MovePlayerToCam();
    void ToggleFreezeWorld(bool isEnabled);
    void ToggleDrawHitbox(bool isEnabled);
    void ToggleWorldHitDraw(int offset, bool isEnabled);
    void ToggleFullShopLineup(bool isEnabled);
    void SetColDrawMode(int val);
    void PatchDebugFont();
    void TogglePlayerSound(bool isEnabled);
}