using System;
using System.Numerics;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class UtilityService(IMemoryService memoryService, HookManager hookManager, IPlayerService playerService)
        : IUtilityService
    {
        public const float DefaultNoClipSpeedScale = 0.2f;

        public void ForceSave() =>
            memoryService.Write(memoryService.Read<nint>(GameMan.Base) + GameMan.ForceSave, (byte)1);

        public void TriggerNewNgCycle() =>
            memoryService.Write(memoryService.Read<nint>(GameMan.Base) + GameMan.ShouldStartNewGame, (byte)1);

        public void ToggleCombatMap(bool isEnabled)
        {
            memoryService.Write(Patches.OpenMap, isEnabled ? (byte)0xEB : (byte)0x74);
            memoryService.WriteBytes(Patches.CloseMap, isEnabled ? [0x90, 0x90, 0x90] : [0xff, 0x50, 0x60]);
        }

        public void ToggleDungeonWarp(bool isEnabled) =>
            memoryService.WriteBytes(Patches.CanFastTravel,
                isEnabled ? [0xB0, 0x01, 0x90, 0x90, 0x90] : [0x84, 0xC0, 0x0F, 0x94, 0xC0]);

        public void ToggleNoClip(bool isNoClipEnabled, bool isKeyboardHookDisabled)
        {
            var inAirTimerCode = CodeCaveOffsets.Base + CodeCaveOffsets.InAirTimer;
            var kbCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.Kb;
            var triggersCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.Triggers;
            var updateCoordsCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords;
            var jumpInterceptCode = CodeCaveOffsets.Base + CodeCaveOffsets.NoClipJmpHook;

            if (isNoClipEnabled)
            {
                WriteInAirTimer(inAirTimerCode);
                if (!isKeyboardHookDisabled) WriteKbCode(kbCode);
                WriteTriggerCode(triggersCode);
                WriteUpdateCoords(updateCoordsCode);
                WriteJumpIntercept(jumpInterceptCode);

                hookManager.InstallHook(inAirTimerCode.ToInt64(), Hooks.InAirTimer, new byte[]
                    { 0xF3, 0x0F, 0x11, 0x43, 0x18 });
                if (!isKeyboardHookDisabled)
                {
                    hookManager.InstallHook(kbCode.ToInt64(), Hooks.NoClipKb, new byte[]
                        { 0xF6, 0x84, 0x08, 0xE8, 0x07, 0x00, 0x00, 0x80 });
                }

                hookManager.InstallHook(triggersCode.ToInt64(), Hooks.NoClipTriggers, new byte[]
                    { 0x0F, 0xB6, 0x44, 0x24, 0x36 });
                hookManager.InstallHook(updateCoordsCode.ToInt64(), Hooks.UpdateCoords, new byte[]
                    { 0x0F, 0x11, 0x43, 0x70, 0xC7, 0x43, 0x7C, 0x00, 0x00, 0x80, 0x3F });
                hookManager.InstallHook(jumpInterceptCode.ToInt64(), Hooks.SetActionRequested,
                    [0x49, 0x09, 0x41, 0x10, 0xC3]);
            }
            else
            {
                hookManager.UninstallHook(inAirTimerCode.ToInt64());
                hookManager.UninstallHook(kbCode.ToInt64());
                hookManager.UninstallHook(triggersCode.ToInt64());
                hookManager.UninstallHook(updateCoordsCode.ToInt64());
                hookManager.UninstallHook(jumpInterceptCode.ToInt64());

                playerService.ToggleNoGravity(false);
            }
        }

        private void WriteInAirTimer(IntPtr inAirTimerCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_InAirTimer);
            var bytes = BitConverter.GetBytes(WorldChrMan.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);

            AsmHelper.WriteImmediateDwords(codeBytes, new[] { (WorldChrMan.PlayerIns, 0xB + 3) });

            AsmHelper.WriteJumpOffsets(codeBytes, new[]
            {
                (Hooks.InAirTimer, 5, inAirTimerCode + 0x28, 0x28 + 1),
            });
            memoryService.WriteBytes(inAirTimerCode, codeBytes);
        }

        private void WriteKbCode(IntPtr kbCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_Keyboard);
            var zDirection = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (kbCode.ToInt64() + 0x10, zDirection.ToInt64(), 7, 0x10 + 2),
                (kbCode.ToInt64() + 0x29, zDirection.ToInt64(), 7, 0x29 + 2),
                (kbCode.ToInt64() + 0x34, Hooks.NoClipKb + 0x8, 5, 0x34 + 1),
                (kbCode.ToInt64() + 0x41, Hooks.NoClipKb + 0x8, 5, 0x41 + 1),
            });

            memoryService.WriteBytes(kbCode, codeBytes);
        }

        private void WriteTriggerCode(IntPtr triggersCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_Triggers);
            var zDirection = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (triggersCode.ToInt64() + 0x7, zDirection.ToInt64(), 7, 0x7 + 2),
                (triggersCode.ToInt64() + 0x1C, zDirection.ToInt64(), 7, 0x1C + 2),
                (triggersCode.ToInt64() + 0x2D, Hooks.NoClipTriggers + 0x5, 5, 0x2D + 1),
            });
            memoryService.WriteBytes(triggersCode, codeBytes);
        }

        private void WriteUpdateCoords(IntPtr updateCoordsCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_UpdateCoords);
            var zDirection = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            var speedScale = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.SpeedScale;

            AsmHelper.WriteImmediateDwords(codeBytes, new[] { (WorldChrMan.PlayerIns, 0xF + 3) });

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (updateCoordsCode.ToInt64() + 0x8, WorldChrMan.Base.ToInt64(), 7, 0x8 + 3),
                (updateCoordsCode.ToInt64() + 0x43, Functions.ChrInsByHandle, 5, 0x43 + 1),
                (updateCoordsCode.ToInt64() + 0x7F, FD4PadManager.Base.ToInt64(), 7, 0x7F + 3),
                (updateCoordsCode.ToInt64() + 0x98, Functions.GetMovement, 5, 0x98 + 1),
                (updateCoordsCode.ToInt64() + 0xAA, Functions.GetMovement, 5, 0xAA + 1),
                (updateCoordsCode.ToInt64() + 0xBC, Functions.GetMovement, 5, 0xBC + 1),
                (updateCoordsCode.ToInt64() + 0xCE, Functions.GetMovement, 5, 0xCE + 1),
                (updateCoordsCode.ToInt64() + 0xF7, FieldArea.Base.ToInt64(), 7, 0xF7 + 3),
                (updateCoordsCode.ToInt64() + 0x113, Functions.MatrixVectorProduct, 5, 0x113 + 1),
                (updateCoordsCode.ToInt64() + 0x132, speedScale.ToInt64(), 9, 0x132 + 5),
                (updateCoordsCode.ToInt64() + 0x148, zDirection.ToInt64(), 6, 0x148 + 2),
                (updateCoordsCode.ToInt64() + 0x172, zDirection.ToInt64(), 7, 0x172 + 2),
                (updateCoordsCode.ToInt64() + 0x1AA, Hooks.UpdateCoords + 0xB, 5, 0x1AA + 1)
            });
            memoryService.WriteBytes(updateCoordsCode, codeBytes);
        }

        public void ToggleNoclipKeyboardHook(bool isEnabled)
        {
            var kbCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.Kb;
            if (!isEnabled)
            {
                WriteKbCode(kbCode);
                hookManager.InstallHook(kbCode.ToInt64(), Hooks.NoClipKb, new byte[]
                    { 0xF6, 0x84, 0x08, 0xE8, 0x07, 0x00, 0x00, 0x80 });
            }
            else
            {
                hookManager.UninstallHook(kbCode.ToInt64());
            }
        }

        public void ToggleGuaranteedDrop(bool isEnabled) =>
            memoryService.WriteBytes(Patches.GetItemChance,
                isEnabled ? [0x66, 0xB8, 0x01, 0x00] : [0x41, 0x0F, 0xB7, 0xC0]);

        public void ToggleDrawPoiseBars(bool isEnabled)
        {
            var worldChrManDbg = memoryService.Read<nint>(WorldChrManDbg.Base);
            memoryService.Write(worldChrManDbg + WorldChrManDbg.PoiseBarsFlag, isEnabled);
        }

        public void SetFps(int fps) =>
            memoryService.Write(Patches.FpsCap + 0x3, 1f / fps);

        public int GetFps() =>
            (int)Math.Round(1f / memoryService.Read<float>(Patches.FpsCap + 0x3));

        private void WriteJumpIntercept(IntPtr jumpInterceptCode)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_JumpHook);
            memoryService.WriteBytes(jumpInterceptCode, bytes);
        }

        public void WriteNoClipSpeed(float speedMultiplier)
        {
            var speedScale = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.SpeedScale;
            memoryService.Write(speedScale, DefaultNoClipSpeedScale * speedMultiplier);
        }

        public float GetSpeed() =>
            memoryService.Read<float>(memoryService.Read<nint>(CSFlipperImp.Base) + CSFlipperImp.GameSpeed);

        public void SetSpeed(float speed) =>
            memoryService.Write(memoryService.Read<nint>(CSFlipperImp.Base) + CSFlipperImp.GameSpeed,
                speed);

        public void ToggleFreeCam(bool isEnabled)
        {
            var patchLoc = Patches.EnableFreeCam;
            var camMode = memoryService.FollowPointers(FieldArea.Base, [FieldArea.GameRend, FieldArea.CamMode], false);
            if (isEnabled)
            {
                memoryService.WriteBytes(patchLoc, [0xB0, 0x01, 0xC3]);
                memoryService.Write(camMode, (byte)1);
            }
            else
            {
                memoryService.WriteBytes(patchLoc, [0xEB, 0x80, 0xCC]);
                memoryService.Write(camMode, (byte)0);
            }
        }

        public void TogglePlayerMovementForFreeCam(bool isEnabled)
        {
            var camMode = memoryService.FollowPointers(FieldArea.Base, [FieldArea.GameRend, FieldArea.CamMode], false);
            memoryService.Write(camMode, isEnabled ? (byte)3 : (byte)1);
        }

        public void MoveCamToPlayer()
        {
            var playerLoc = playerService.IsRiding() ? playerService.GetTorrentPos() : playerService.GetPlayerPos();

            playerLoc.Y += 2.5f;
            memoryService.Write(GetDbgCamCoordsPtr(), playerLoc);
        }

        public void MovePlayerToCam()
        {
            var cameraLoc = memoryService.Read<Vector3>(GetDbgCamCoordsPtr());

            if (playerService.IsRiding())
            {
                playerService.SetTorrentPos(cameraLoc);
            }
            else
            {
                playerService.SetPlayerPos(cameraLoc);
            }
        }

        public void ToggleFreezeWorld(bool isEnabled) =>
            memoryService.WriteBytes(Patches.IsWorldPaused, isEnabled ? [0x0F, 0x85] : [0x0F, 0x84]);

        public void ToggleDrawHitbox(bool isDrawHitboxEnabled) =>
            memoryService.Write(memoryService.Read<nint>(DamageManager.Base) + DamageManager.HitboxView,
                isDrawHitboxEnabled);

        public void ToggleDrawRagdolls(bool isEnabled) =>
            memoryService.Write(WorldHitMan.Base + WorldHitMan.Ragdoll, isEnabled);

        public void ToggleDrawLowHit(bool isEnabled)
        {
            memoryService.Write(WorldHitMan.Base + WorldHitMan.LowHit, isEnabled);
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowGeom,
                isEnabled ? (byte)0 : (byte)1);
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowMap,
                isEnabled ? (byte)0 : (byte)1);
        }

        public void ToggleDrawHighHit(bool isEnabled)
        {
            memoryService.Write(WorldHitMan.Base + WorldHitMan.HighHit, isEnabled);
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowGeom,
                isEnabled ? (byte)0 : (byte)1);
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowMap,
                isEnabled ? (byte)0 : (byte)1);
        }

        public void ToggleFullShopLineup(bool isEnabled) =>
            memoryService.WriteBytes(Patches.GetShopEvent,
                isEnabled ? [0xB0, 0x01, 0xC3, 0x90, 0x90, 0x90] : [0x40, 0x53, 0x48, 0x83, 0xEC, 0x40]);

        public void SetColDrawMode(int val) =>
            memoryService.Write(WorldHitMan.Base + WorldHitMan.Mode, (byte)val);

        public void PatchDebugFont() => memoryService.Write(Patches.DebugFont, (byte)0xC3);

        public void TogglePlayerSound(bool isEnabled) =>
            memoryService.Write(Patches.PlayerSound, isEnabled ? (byte)0x75 : (byte)0x74);

        public void ToggleDrawMapTiles1(bool isEnabled)
        {
            var ptr = memoryService.Read<nint>(FieldArea.Base) + FieldArea.DrawTiles1;
            memoryService.Write(ptr, isEnabled);
        }

        public void ToggleDrawMapTiles2(bool isEnabled)
        {
            var ptr = memoryService.Read<nint>(FieldArea.Base) + FieldArea.DrawTiles2;
            memoryService.Write(ptr, isEnabled);
        }

        public void ToggleDrawMiniMap(bool isEnabled)
        {
            var ptr = memoryService.FollowPointers(FieldArea.Base,
                [FieldArea.WorldInfoOwner, FieldArea.WorldInfoOwnerOffsets.ShouldDrawMiniMap], false);
            memoryService.Write(ptr, isEnabled);
        }

        public void ToggleHideChr(bool isEnabled) =>
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowChrs,
                isEnabled ? (byte)0 : (byte)1);

        public void ToggleHideMap(bool isEnabled)
        {
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowGeom,
                isEnabled ? (byte)0 : (byte)1);
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowMap,
                isEnabled ? (byte)0 : (byte)1);
            memoryService.Write(GroupMask.Base + (int)GroupMask.GroupMasks.ShouldShowMap2,
                isEnabled ? (byte)0 : (byte)1);
        }

        public void ToggleDrawTilesOnMap(bool isEnabled) =>
            memoryService.Write(MapDebugFlags.Base + MapDebugFlags.ShowMapTiles, isEnabled);

        private IntPtr GetDbgCamCoordsPtr() =>
            memoryService.FollowPointers(FieldArea.Base,
                [FieldArea.GameRend, FieldArea.CSDebugCam, FieldArea.CamCoords], false);
    }
}