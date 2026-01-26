// 

namespace TarnishedTool.Interfaces;

public interface IUtilityService
{
    void ForceSave();
    void TriggerNewNgCycle();
    void ToggleCombatMap(bool isEnabled);
    void ToggleDungeonWarp(bool isEnabled);
    void ToggleNoClip(bool isEnabled, bool isKeyboardHookDisabled);
    void WriteNoClipSpeed(float speedMultiplier);
    float GetSpeed();
    void SetSpeed(float speed);
    void ToggleFreeCam(bool isEnabled);
    void TogglePlayerMovementForFreeCam(bool isEnabled);
    void MoveCamToPlayer();
    void MovePlayerToCam();
    void ToggleFreezeWorld(bool isEnabled);
    void ToggleDrawHitbox(bool isEnabled);
    void ToggleDrawRagdolls(bool isEnabled);
    void ToggleDrawLowHit(bool isEnabled);
    void ToggleDrawHighHit(bool isEnabled);
    void ToggleFullShopLineup(bool isEnabled);
    void SetColDrawMode(int val);
    void PatchDebugFont();
    void TogglePlayerSound(bool isEnabled);
    void ToggleDrawMapTiles1(bool isEnabled);
    void ToggleDrawMapTiles2(bool isEnabled);
    void ToggleDrawMiniMap(bool isEnabled);
    void ToggleHideChr(bool isEnabled);
    void ToggleHideMap(bool isEnabled);
    void ToggleDrawTilesOnMap(bool isEnabled);
    void ToggleNoclipKeyboardHook(bool isEnabled);
    void ToggleGuaranteedDrop(bool isGuaranteedDropEnabled);
    void ToggleDrawPoiseBars(bool isDrawPoiseBarsEnabled);
    void SetFps(int fps);
    int GetFps();
}