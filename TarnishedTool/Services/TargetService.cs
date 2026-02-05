using System;
using System.Numerics;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class TargetService(
        IMemoryService memoryService,
        HookManager hookManager,
        IPlayerService playerService,
        IReminderService reminderService,
        IChrInsService chrInsService)
        : ITargetService
    {
        public void ToggleTargetHook(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.SaveTargetPtrCode;
            if (isEnabled)
            {
                reminderService.TrySetReminder();
                var hook = Hooks.LockedTargetPtr;
                var savedPtr = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                var bytes = AsmLoader.GetAsmBytes("LockedTarget");
                AsmHelper.WriteRelativeOffsets(bytes, [
                    (code.ToInt64() + 0x7, savedPtr.ToInt64(), 7, 0x7 + 3),
                    (code.ToInt64() + 0xE, hook + 0x7, 5, 0xE + 1)
                ]);
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hook,
                    [0x48, 0x8B, 0x8F, 0x88, 0x00, 0x00, 0x00]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public nint GetTargetChrIns() =>
            memoryService.Read<nint>(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr);

        public void SetHp(int health) => chrInsService.SetHp(GetTargetChrIns(), health);
        public int GetCurrentHp() => chrInsService.GetCurrentHp(GetTargetChrIns());
        public int GetMaxHp() => chrInsService.GetMaxHp(GetTargetChrIns());
        public float GetCurrentPoise() => chrInsService.GetCurrentPoise(GetTargetChrIns());
        public float GetMaxPoise() => chrInsService.GetMaxPoise(GetTargetChrIns());
        public float GetPoiseTimer() => chrInsService.GetPoiseTimer(GetTargetChrIns());
        public Vector3 GetLocalCoords() => chrInsService.GetLocalCoords(GetTargetChrIns());
        public float GetSpeed() => chrInsService.GetSpeed(GetTargetChrIns());
        public void SetSpeed(float speed) => chrInsService.SetSpeed(GetTargetChrIns(), speed);

        public void ToggleTargetAi(bool isDisableTargetAiEnabled) =>
            chrInsService.ToggleTargetAi(GetTargetChrIns(), isDisableTargetAiEnabled);

        public bool IsAiDisabled() => chrInsService.IsAiDisabled(GetTargetChrIns());

        public void ToggleNoAttack(bool isNoAttackEnabled) =>
            chrInsService.ToggleNoAttack(GetTargetChrIns(), isNoAttackEnabled);

        public bool IsNoAttackEnabled() => chrInsService.IsNoAttackEnabled(GetTargetChrIns());

        public void ToggleNoMove(bool isNoMoveEnabled) =>
            chrInsService.ToggleNoMove(GetTargetChrIns(), isNoMoveEnabled);

        public bool IsNoMoveEnabled() => chrInsService.IsNoMoveEnabled(GetTargetChrIns());

        public void ForceAct(int act) =>
            memoryService.WriteUInt8(GetAiThinkPtr() + ChrIns.AiThinkOffsets.ForceAct, act);

        public int GetLastAct() =>
            memoryService.ReadUInt8(GetAiThinkPtr() + ChrIns.AiThinkOffsets.LastAct);

        public int GetForceAct() =>
            memoryService.ReadUInt8(GetAiThinkPtr() + ChrIns.AiThinkOffsets.ForceAct);

        public void ToggleRepeatAct(bool isRepeatActEnabled)
        {
            var ptr = GetAiThinkPtr() + ChrIns.AiThinkOffsets.ForceAct;
            memoryService.WriteUInt8(ptr, isRepeatActEnabled ? memoryService.ReadUInt8(ptr + 1) : 0);
        }

        public bool IsTargetRepeating() =>
            memoryService.ReadUInt8(GetAiThinkPtr() + ChrIns.AiThinkOffsets.ForceAct) != 0;

        public int GetCurrentAnimation() => chrInsService.GetCurrentAnimation(GetTargetChrIns());
        
        public void ToggleTargetingView(bool isTargetingViewEnabled) => 
            chrInsService.ToggleTargetView(GetTargetChrIns(), isTargetingViewEnabled);

        public bool IsTargetViewEnabled() => chrInsService.IsTargetViewEnabled(GetTargetChrIns());

        public void ToggleTargetNoDamage(bool isNoDamageEnabled) => 
            chrInsService.ToggleNoDamage(GetTargetChrIns(), isNoDamageEnabled);

        public bool IsNoDamageEnabled() => chrInsService.IsNoDamageEnabled(GetTargetChrIns());

        public void ToggleNoStagger(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.TargetNoStagger;
            if (isEnabled)
            {
                var hookLoc = Hooks.TargetNoStagger;
                var lockedTarget = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                var bytes = AsmLoader.GetAsmBytes("TargetNoStagger");
                AsmHelper.WriteRelativeOffsets(bytes, [
                    (code.ToInt64() + 0x5, lockedTarget.ToInt64(), 7, 0x5 + 3),
                    (code.ToInt64() + 0x17, hookLoc + 8, 5, 0x17 + 1)
                ]);
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc,
                    [0x48, 0x8B, 0x41, 0x08, 0x83, 0x48, 0x2C, 0x08]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void KillAllBesidesTarget()
        {
            var worldChrMan = memoryService.ReadInt64(WorldChrMan.Base);
            var lockedTarget =
                memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr);
            var bytes = AsmLoader.GetAsmBytes("KillAll");

            AsmHelper.WriteImmediateDwords(bytes, [
                (WorldChrMan.PlayerIns, 0x18 + 3),
                (WorldChrMan.ChrInsByUpdatePrioBegin, 0x27 + 3),
                (WorldChrMan.ChrInsByUpdatePrioEnd, 0x2E + 3)
            ]);

            AsmHelper.WriteAbsoluteAddresses(bytes, [
                (lockedTarget, 0x4 + 2),
                (worldChrMan, 0xE + 2)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }

        public void ToggleDisableAllExceptTarget(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.DisableAllExceptTarget;
            if (isEnabled)
            {
                var lockedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                var hookLoc = Hooks.ShouldUpdateAi;
                var bytes = AsmLoader.GetAsmBytes("DisableAllExceptTarget");

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (code.ToInt64() + 0x5, lockedTargetPtr.ToInt64(), 7, 0x5 + 3),
                    (code.ToInt64() + 0x14, hookLoc + 0x5, 5, 0x14 + 1)
                ]);

                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc, [0x48, 0x89, 0x5C, 0x24, 0x08]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public int GetNpcThinkParamId() => chrInsService.GetNpcThinkParamId(GetTargetChrIns());
        public int GetNpcChrId() => chrInsService.GetChrId(GetTargetChrIns());
        public uint GetNpcParamId() => chrInsService.GetNpcParamId(GetTargetChrIns());

        public void ToggleNoHeal(bool isNoHealEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoHeal;
            if (isNoHealEnabled)
            {
                var hook = Hooks.NoHeal;
                var codeBytes = AsmLoader.GetAsmBytes("NoHeal");
                var target = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                AsmHelper.WriteRelativeOffsets(codeBytes, [
                    (code.ToInt64() + 5, target.ToInt64(), 7, 5 + 3),
                    (code.ToInt64() + 0x24, hook + 6, 5, 0x24 + 1)
                ]);
                memoryService.WriteBytes(code, codeBytes);
                hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x89, 0x81, 0x38, 0x01, 0x00, 0x00 });
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public int GetResistance(int offset) => chrInsService.GetResistance(GetTargetChrIns(), offset);
        public bool[] GetImmunities() => chrInsService.GetImmunities(GetTargetChrIns());
        public float[] GetDefenses() => chrInsService.GetDefenses(GetTargetChrIns());
        public float GetDist() => chrInsService.GetDistBetweenChrs(playerService.GetPlayerIns(), GetTargetChrIns());
        public uint GetEntityId() => chrInsService.GetEntityId(GetTargetChrIns());
        

        public IntPtr GetAiThinkPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.AiThink], true);
        
    }
}