using System;
using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class TargetService(MemoryService memoryService, HookManager hookManager) : ITargetService
    {
        public void ToggleTargetHook(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.Code;
            if (isEnabled)
            {
                var hook = Hooks.LockedTargetPtr;
                var savedPtr = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr;
                var bytes = AsmLoader.GetAsmBytes("LockedTarget");
                AsmHelper.WriteRelativeOffsets(bytes, new[]
                {
                    (code.ToInt64() + 0x7, savedPtr.ToInt64(), 7, 0x7 + 3),
                    (code.ToInt64() + 0xE, hook + 0x7, 5, 0xE + 1)
                });
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x48, 0x8B, 0x8F, 0x88, 0x00, 0x00, 0x00 });
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public ulong GetTargetAddr() =>
            memoryService.ReadUInt64(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr);

        public void SetHp(int health) =>
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health, health);

        public int GetCurrentHp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health);

        public int GetMaxHp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHealth);

        public float[] GetPosition()
        {
            var posPtr = memoryService.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                [..ChrIns.ChrPhysicsModule, (int)ChrIns.ChrPhysicsOffsets.Coords], false);

            float[] position = new float[3];
            position[0] = memoryService.ReadFloat(posPtr);
            position[1] = memoryService.ReadFloat(posPtr + 0x4);
            position[2] = memoryService.ReadFloat(posPtr + 0x8);

            return position;
        }

        public float GetSpeed() =>
            memoryService.ReadFloat(GetChrBehaviorPtr() + (int)ChrIns.ChrBehaviorOffsets.AnimSpeed);

        public void SetSpeed(float speed) =>
            memoryService.WriteFloat(GetChrBehaviorPtr() + (int)ChrIns.ChrBehaviorOffsets.AnimSpeed, speed);

        public void ToggleTargetAi(bool isDisableTargetAiEnabled) =>
            memoryService.SetBitValue(GetChrFlagsPtr() + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit,
                isDisableTargetAiEnabled);

        public bool IsAiDisabled() =>
            memoryService.IsBitSet(GetChrFlagsPtr() + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit);


        public void ForceAct(int act) =>
            memoryService.WriteUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceAct, act);

        public int GetLastAct() =>
            memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.LastAct);

        public int GetForceAct() =>
            memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceAct);

        public void ToggleRepeatAct(bool isRepeatActEnabled)
        {
            var ptr = GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceAct;
            memoryService.WriteUInt8(ptr, isRepeatActEnabled ? memoryService.ReadUInt8(ptr + 1) : 0);
        }

        public bool IsTargetRepeating() =>
            memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceAct) != 0;

        public void ToggleTargetingView(bool isTargetingViewEnabled)
        {
            var targetingSystem =
                memoryService.ReadInt64(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.TargetingSystem);
            memoryService.SetBitValue((IntPtr)targetingSystem + ChrIns.BlueTargetView.Offset, ChrIns.BlueTargetView.Bit,
                isTargetingViewEnabled);
        }

        public bool IsTargetViewEnabled()
        {
            var targetingSystem =
                memoryService.ReadInt64(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.TargetingSystem);
            return memoryService.IsBitSet((IntPtr)targetingSystem + ChrIns.BlueTargetView.Offset,
                ChrIns.BlueTargetView.Bit);
        }

        public void ToggleTargetNoDamage(bool isFreezeHealthEnabled)
        {
            var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Flags;
            memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage, isFreezeHealthEnabled);
        }

        public bool IsTargetNoDamageEnabled()
        {
            var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Flags;
            return memoryService.IsBitSet(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage);
        }

        public void KillAllBesidesTarget()
        {
            var worldChrMan = memoryService.ReadInt64(WorldChrMan.Base);
            var lockedTarget =
                memoryService.ReadInt64(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr);
            var bytes = AsmLoader.GetAsmBytes("KillAll");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (lockedTarget, 0x4 + 2),
                (worldChrMan, 0xE + 2),
            });
            
            memoryService.AllocateAndExecute(bytes);
        }

        public void ToggleDisableAllExceptTarget(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.DisableAllExceptTarget;
            if (isEnabled)
            {
                var lockedTargetPtr = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr;
                var hookLoc = Hooks.ShouldUpdateAi;
                var bytes = AsmLoader.GetAsmBytes("DisableAllExceptTarget");
                AsmHelper.WriteRelativeOffsets(bytes, new []
                {
                    (code.ToInt64() + 0x5, lockedTargetPtr.ToInt64(), 7, 0x5 + 3),
                    (code.ToInt64() + 0x14, hookLoc + 0x5, 5, 0x14 + 1)
                });

                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc, [0x48, 0x89, 0x5C, 0x24, 0x08]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public int GetNpcThinkParamId() =>
            memoryService.ReadInt32(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.NpcThinkParamId);

        private IntPtr GetChrFlagsPtr()
        {
            return memoryService.FollowPointers(
                CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                [ChrIns.ChrCtrl, ..ChrIns.ChrCtrlFlags], false
            );
        }

        private IntPtr GetChrDataPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                [..ChrIns.ChrDataModule], true);

        private IntPtr GetChrBehaviorPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                [..ChrIns.ChrBehaviorModule], true);


        private IntPtr GetAiThinkPtr()
        {
            return memoryService.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr, [
                ..ChrIns.AiThink,
            ], true);
        }
    }
}