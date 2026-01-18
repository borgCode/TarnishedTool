using System;
using System.Numerics;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class TargetService(
        MemoryService memoryService,
        HookManager hookManager,
        IPlayerService playerService,
        IReminderService reminderService)
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
                AsmHelper.WriteRelativeOffsets(bytes, new[]
                {
                    (code.ToInt64() + 0x7, savedPtr.ToInt64(), 7, 0x7 + 3),
                    (code.ToInt64() + 0xE, hook + 0x7, 5, 0xE + 1)
                });
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hook,
                    [0x48, 0x8B, 0x8F, 0x88, 0x00, 0x00, 0x00]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        public long GetTargetChrIns() =>
            memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr);

        public void SetHp(int health) =>
            memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health, health);

        public int GetCurrentHp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Health);

        public int GetMaxHp() =>
            memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHealth);

        public float GetCurrentPoise() =>
            memoryService.ReadFloat(GetChrSuperArmorPtr() + (int)ChrIns.ChrSuperArmorOffsets.CurrentPoise);

        public float GetMaxPoise() =>
            memoryService.ReadFloat(GetChrSuperArmorPtr() + (int)ChrIns.ChrSuperArmorOffsets.MaxPoise);

        public float GetPoiseTimer() =>
            memoryService.ReadFloat(GetChrSuperArmorPtr() + (int)ChrIns.ChrSuperArmorOffsets.PoiseTimer);

        public float[] GetPosition()
        {
            var posPtr = GetChrPhysPtr() + (int)ChrIns.ChrPhysicsOffsets.Coords;

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
            memoryService.SetBitValue(GetChrCtrlFlagsPtr() + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit,
                isDisableTargetAiEnabled);

        public bool IsAiDisabled() =>
            memoryService.IsBitSet(GetChrCtrlFlagsPtr() + ChrIns.DisableAi.Offset, ChrIns.DisableAi.Bit);

        public void ToggleNoAttack(bool isNoAttackEnabled) =>
            memoryService.SetBitValue(GetChrInsFlagsPtr(), (int)ChrIns.ChrInsFlags.NoAttack, isNoAttackEnabled);

        public bool IsNoAttackEnabled() =>
            memoryService.IsBitSet(GetChrInsFlagsPtr(), (int)ChrIns.ChrInsFlags.NoAttack);

        public void ToggleNoMove(bool isNoMoveEnabled) =>
            memoryService.SetBitValue(GetChrInsFlagsPtr(), (int)ChrIns.ChrInsFlags.NoMove, isNoMoveEnabled);

        public bool IsNoMoveEnabled() =>
            memoryService.IsBitSet(GetChrInsFlagsPtr(), (int)ChrIns.ChrInsFlags.NoMove);

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

        public int GetCurrentAnimation() =>
            memoryService.ReadInt32(GetChrTimeActPtr() + (int)ChrIns.ChrTimeActOffsets.AnimationId);

        public void SetAnimation(int animationId) =>
            memoryService.WriteInt32(GetAiThinkPtr() + ChrIns.AiThinkOffsets.AnimationRequest, animationId);

        public void ToggleTargetingView(bool isTargetingViewEnabled)
        {
            var targetingSystem =
                memoryService.ReadInt64(GetAiThinkPtr() + ChrIns.AiThinkOffsets.TargetingSystem);
            var flags = targetingSystem + (int)ChrIns.TargetingSystemOffsets.DebugDrawFlags;
            memoryService.SetBitValue((IntPtr)flags + ChrIns.BlueTargetView.Offset, ChrIns.BlueTargetView.Bit,
                isTargetingViewEnabled);
        }

        public bool IsTargetViewEnabled()
        {
            var targetingSystem =
                memoryService.ReadInt64(GetAiThinkPtr() + ChrIns.AiThinkOffsets.TargetingSystem);
            var flags = targetingSystem + (int)ChrIns.TargetingSystemOffsets.DebugDrawFlags;
            return memoryService.IsBitSet((IntPtr)flags + ChrIns.BlueTargetView.Offset,
                ChrIns.BlueTargetView.Bit);
        }

        public void ToggleTargetNoDamage(bool isFreezeHealthEnabled)
        {

            var bitFlags = GetChrDataPtr() + ChrIns.ChrDataFlags;
            memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage, isFreezeHealthEnabled);
            
        }

        public bool IsNoDamageEnabled() 
        {
            var bitFlags = GetChrDataPtr() + ChrIns.ChrDataFlags;
            return memoryService.IsBitSet(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage);
        }

        public void ToggleNoStagger(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.TargetNoStagger;
            if (isEnabled)
            {
                var hookLoc = Hooks.TargetNoStagger;
                var lockedTarget = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                var bytes = AsmLoader.GetAsmBytes("TargetNoStagger");
                AsmHelper.WriteRelativeOffsets(bytes, new[]
                {
                    (code.ToInt64() + 0x5, lockedTarget.ToInt64(), 7, 0x5 + 3),
                    (code.ToInt64() + 0x17, hookLoc + 8, 5, 0x17 + 1)
                });
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

            AsmHelper.WriteImmediateDwords(bytes, new[]
            {
                (WorldChrMan.PlayerIns, 0x18 + 3),
                (WorldChrMan.ChrInsByUpdatePrioBegin, 0x27 + 3),
                (WorldChrMan.ChrInsByUpdatePrioEnd, 0x2E + 3),
            });

            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
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
                var lockedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                var hookLoc = Hooks.ShouldUpdateAi;
                var bytes = AsmLoader.GetAsmBytes("DisableAllExceptTarget");

                AsmHelper.WriteRelativeOffsets(bytes, new[]
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

        public int GetNpcChrId() => 
            memoryService.ReadInt32((IntPtr)GetTargetChrIns() + ChrIns.ChrId);

        public void ToggleNoHeal(bool isNoHealEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoHeal;
            if (isNoHealEnabled)
            {
                var hook = Hooks.NoHeal;
                var codeBytes = AsmLoader.GetAsmBytes("NoHeal");
                var target = CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr;
                AsmHelper.WriteRelativeOffsets(codeBytes,[
                (code.ToInt64() + 5, target.ToInt64(), 7, 5+3), 
                (code.ToInt64() + 0x24, hook + 6, 5, 0x24+1)
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
        

        public int GetResistance(int offset) =>
            memoryService.ReadInt32(GetChrResistPtr() + offset);

        public bool[] GetImmunities()
        {
            var ptr = GetNpcParamPtr();
            var immunities = new bool[5];
            immunities[0] = memoryService.ReadInt32(ptr + (int)ChrIns.NpcParamOffsets.SleepImmune) == 90300;
            immunities[1] = memoryService.ReadInt32(ptr + (int)ChrIns.NpcParamOffsets.PoisonImmune) == 90000;
            immunities[2] = memoryService.ReadInt32(ptr + (int)ChrIns.NpcParamOffsets.RotImmune) == 90010;
            immunities[4] = memoryService.ReadInt32(ptr + (int)ChrIns.NpcParamOffsets.FrostImmune) == 90040;
            immunities[3] = memoryService.ReadInt32(ptr + (int)ChrIns.NpcParamOffsets.BleedImmune) == 90020;
            return immunities;
        }

        public float[] GetDefenses()
        {
            var ptr = GetNpcParamPtr();
            var defenses = new float[8];
            defenses[0] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.StandardAbsorption);
            defenses[1] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.SlashAbsorption);
            defenses[2] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.StrikeAbsorption);
            defenses[3] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.ThrustAbsorption);
            defenses[4] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.MagicAbsorption);
            defenses[5] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.FireAbsorption);
            defenses[6] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.LightningAbsorption);
            defenses[7] = memoryService.ReadFloat(ptr + (int)ChrIns.NpcParamOffsets.HolyAbsorption);
            return defenses;
        }

        public float GetDist()
        {
            var playerPos = playerService.GetPosWithHurtbox();
            var targetPos = GetPosWithHurtbox();

            float distance = Vector3.Distance(playerPos.position, targetPos.position);
            return distance - targetPos.capsuleRadius - playerPos.capsuleRadius;
        }

        public uint GetEntityId() =>
            memoryService.ReadUInt32((IntPtr)memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr) +
                                     ChrIns.EntityId);

        private IntPtr GetChrCtrlFlagsPtr() =>
            memoryService.FollowPointers(
                CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [ChrIns.ChrCtrl, ..ChrIns.ChrCtrlFlags], false);

        private IntPtr GetChrInsFlagsPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr, [ChrIns.Flags], false);

        private nint GetChrResistPtr() =>
            memoryService.FollowPointers(
                CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.ChrResistModule], true);

        private IntPtr GetChrDataPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.ChrDataModule], true);

        private IntPtr GetChrTimeActPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.ChrTimeActModule], true);

        private IntPtr GetChrBehaviorPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.ChrBehaviorModule], true);

        public IntPtr GetChrSuperArmorPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.ChrSuperArmorModule], true);

        private IntPtr GetAiThinkPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.AiThink], true);

        private IntPtr GetNpcParamPtr() =>
            memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
                [..ChrIns.NpcParam], true);

        private IntPtr GetChrPhysPtr() => memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.TargetPtr,
            [..ChrIns.ChrPhysicsModule], true);

        private PosWithHurtbox GetPosWithHurtbox()
        {
            var physPtr = GetChrPhysPtr();
            var position = memoryService.ReadVector3(physPtr + (int)ChrIns.ChrPhysicsOffsets.Coords);
            var capsuleRadius = memoryService.ReadFloat(physPtr + (int)ChrIns.ChrPhysicsOffsets.HurtCapsuleRadius);
            return new PosWithHurtbox(position, capsuleRadius);
        }
    }
}