using System;
using SilkyRing.Memory;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;
using Array = System.Array;

namespace SilkyRing.Services
{
    public class EnemyService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;

        public EnemyService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }


        public void ToggleRykardMega(bool isRykardNoMegaEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.Rykard;
            if (isRykardNoMegaEnabled)
            {
                var hook = Hooks.HasSpEffect;
                var codeBytes = AsmLoader.GetAsmBytes("RykardNoMega");
                var bytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x17);
                Array.Copy(bytes, 0, codeBytes, 0x12 + 1, 4);
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x48, 0x8B, 0x49, 0x08, 0x48, 0x85, 0xC9 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

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
                _memoryIo.WriteBytes(code, bytes);
                _hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x48, 0x8B, 0x8F, 0x88, 0x00, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public long GetTargetPtr() =>
            _memoryIo.ReadInt64(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr);

        public int GetTargetHp() =>
            _memoryIo.ReadInt32(GetTargetChrDataFieldPtr(WorldChrMan.Offsets.ChrIns.Modules.ChrData.Health));

        public int GetTargetMaxHp() =>
            _memoryIo.ReadInt32(GetTargetChrDataFieldPtr(WorldChrMan.Offsets.ChrIns.Modules.ChrData.MaxHealth));

        public void SetTargetHp(int health) =>
            _memoryIo.WriteInt32(GetTargetChrDataFieldPtr(WorldChrMan.Offsets.ChrIns.Modules.ChrData.Health), health);


        private IntPtr GetTargetChrDataFieldPtr(int fieldOffset)
        {
            return _memoryIo.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                new[]
                {
                    WorldChrMan.Offsets.ChrIns.ModulesPtr,
                    WorldChrMan.Offsets.ChrIns.Modules.ChrDataPtr,
                    fieldOffset
                }, false);
        }

        public float[] GetTargetPos()
        {
            var posPtr = _memoryIo.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                new[]
                {
                    WorldChrMan.Offsets.ChrIns.ModulesPtr,
                    WorldChrMan.Offsets.ChrIns.Modules.ChrPhysicsPtr,
                    WorldChrMan.Offsets.ChrIns.Modules.ChrPhysics.Coords
                }, false
            );

            float[] position = new float[3];
            position[0] = _memoryIo.ReadFloat(posPtr);
            position[1] = _memoryIo.ReadFloat(posPtr + 0x4);
            position[2] = _memoryIo.ReadFloat(posPtr + 0x8);

            return position;
        }

        public void ClearLockedTarget() =>
            _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr, new byte[]
                { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

        public void ToggleTargetAi(bool isDisableTargetAiEnabled) =>
            _memoryIo.SetBitValue(GetChrFlagsPtr(), (byte)WorldChrMan.Offsets.ChrIns.ChrCtrl.Unk.Flag.DisableAi,
                isDisableTargetAiEnabled);

        private IntPtr GetChrFlagsPtr()
        {
            var flagPtr = _memoryIo.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr,
                new[]
                {
                    WorldChrMan.Offsets.ChrIns.ChrCtrlPtr,
                    WorldChrMan.Offsets.ChrIns.ChrCtrl.UnkPtr,
                    WorldChrMan.Offsets.ChrIns.ChrCtrl.Unk.Flags,
                }, false
            );
            return flagPtr;
        }

        public bool IsAiDisabled() =>
            _memoryIo.IsBitSet(GetChrFlagsPtr(), (byte)WorldChrMan.Offsets.ChrIns.ChrCtrl.Unk.Flag.DisableAi);

        
        public void ForceAct(int forceAct) => 
            _memoryIo.WriteUInt8(GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.ForceAct), (byte)forceAct);
        public int GetLastAct() =>
            _memoryIo.ReadUInt8(GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.LastAct));
        public int GetForceAct() =>
            _memoryIo.ReadUInt8(GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.ForceAct));

        private IntPtr GetAiField(int offset) =>
            _memoryIo.FollowPointers(CodeCaveOffsets.Base + (int)CodeCaveOffsets.LockedTarget.SavedPtr, new[]
            {
                WorldChrMan.Offsets.ChrIns.ComManipulatorPtr,
                WorldChrMan.Offsets.ChrIns.ComManipulator.AiPtr,
                offset
            }, false);

        public void ToggleRepeatAct(bool isRepeatActEnabled)
        {
            var ptr = GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.ForceAct);
            _memoryIo.WriteUInt8(ptr, isRepeatActEnabled ? _memoryIo.ReadUInt8(ptr + 1) : (byte)0);
        }
        
        public bool IsTargetRepeating() => 
            _memoryIo.ReadUInt8(GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.ForceAct)) != 0;

        public void ToggleTargetingView(bool isTargetingViewEnabled)
        {
            var targetingSystem = _memoryIo.ReadInt64(GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.TargetingSystemPtr));
            _memoryIo.SetBit32(
                (IntPtr)targetingSystem + WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.TargetingSystem.Flags,
                (int)WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.TargetingSystem.Flag.BlueTargetView,
                isTargetingViewEnabled);
        }

        public bool IsTargetViewEnabled()
        {
            var targetingSystem = _memoryIo.ReadInt64(GetAiField(WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.TargetingSystemPtr));
            return _memoryIo.GetBit32(
                (IntPtr)targetingSystem + WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.TargetingSystem.Flags,
                (int)WorldChrMan.Offsets.ChrIns.ComManipulator.Ai.TargetingSystem.Flag.BlueTargetView);
        }

        public void ToggleTargetNoDamage(bool isFreezeHealthEnabled)
        {
            var chrDataFlags = GetTargetChrDataFieldPtr(WorldChrMan.Offsets.ChrIns.Modules.ChrData.Flags);
            _memoryIo.SetBitValue(chrDataFlags, (byte)WorldChrMan.Offsets.ChrIns.Modules.ChrData.Flag.NoDamage, isFreezeHealthEnabled);
        }

        public bool IsTargetNoDamageEnabled()
        {
            var chrDataFlags = GetTargetChrDataFieldPtr(WorldChrMan.Offsets.ChrIns.Modules.ChrData.Flags);
            return _memoryIo.IsBitSet(chrDataFlags, (byte)WorldChrMan.Offsets.ChrIns.Modules.ChrData.Flag.NoDamage);
        }
    }
}