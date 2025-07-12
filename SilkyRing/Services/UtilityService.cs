using System;
using SilkyRing.Memory;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class UtilityService
    {
        
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        public UtilityService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }

        public void ToggleNoClip(bool isNoClipEnabled)
        {
            var inAirTimerCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.InAirTimer;
            var kbCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.Kb;
            var triggersCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.Triggers;
            var updateCoordsCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords;
            
            if (isNoClipEnabled)
            {
                WriteInAirTimer(inAirTimerCode);
                WriteKbCode(kbCode);
                WriteTriggerCode(triggersCode);
                WriteUpdateCoords(updateCoordsCode);

                _hookManager.InstallHook(inAirTimerCode.ToInt64(), Hooks.InAirTimer, new byte[]
                    { 0xF3, 0x0F, 0x11, 0x43, 0x18 });
                _hookManager.InstallHook(kbCode.ToInt64(), Hooks.NoClipKb, new byte[]
                    { 0xF6, 0x84, 0x08, 0xE8, 0x07, 0x00, 0x00, 0x80 });
                _hookManager.InstallHook(triggersCode.ToInt64(), Hooks.NoClipTriggers, new byte[]
                    { 0x0F, 0xB6, 0x44, 0x24, 0x36 });
                _hookManager.InstallHook(updateCoordsCode.ToInt64(), Hooks.UpdateCoords, new byte[]
                    { 0x0F, 0x11, 0x43, 0x70, 0xC7, 0x43, 0x7C, 0x00, 0x00, 0x80, 0x3F});
            }
            else
            {
                _hookManager.UninstallHook(inAirTimerCode.ToInt64());
                _hookManager.UninstallHook(kbCode.ToInt64());
                _hookManager.UninstallHook(triggersCode.ToInt64());
                _hookManager.UninstallHook(updateCoordsCode.ToInt64());
            }
        }

        private void WriteInAirTimer(IntPtr inAirTimerCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes("NoClip_InAirTimer");
            var bytes = BitConverter.GetBytes(WorldChrMan.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
            AsmHelper.WriteJumpOffsets(codeBytes, new []
            {
                (Hooks.InAirTimer, 5, inAirTimerCode + 0x28, 0x28 + 1),
            });
             _memoryIo.WriteBytes(inAirTimerCode, codeBytes);
        }

        private void WriteKbCode(IntPtr kbCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes("NoClip_Keyboard");
            var zDirection = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            
            AsmHelper.WriteRelativeOffsets(codeBytes, new []
            {
                (kbCode.ToInt64() + 0x10, zDirection.ToInt64(), 7, 0x10 + 2),
                (kbCode.ToInt64() + 0x29, zDirection.ToInt64(), 7, 0x29 + 2),
                (kbCode.ToInt64() + 0x34, Hooks.NoClipKb + 0x8, 5, 0x34 + 1),
                (kbCode.ToInt64() + 0x41, Hooks.NoClipKb + 0x8, 5, 0x41 + 1),
            });
          
            _memoryIo.WriteBytes(kbCode, codeBytes);
        }

        private void WriteTriggerCode(IntPtr triggersCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes("NoClip_Triggers");
            var zDirection = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            AsmHelper.WriteRelativeOffsets(codeBytes, new []
            {
                (triggersCode.ToInt64() + 0x7, zDirection.ToInt64(), 7, 0x7 + 2),
                (triggersCode.ToInt64() + 0x1C, zDirection.ToInt64(), 7, 0x1C + 2),
                (triggersCode.ToInt64() + 0x2D, Hooks.NoClipTriggers + 0x5, 5, 0x2D + 1),
            });
            _memoryIo.WriteBytes(triggersCode, codeBytes);
        }

        private void WriteUpdateCoords(IntPtr updateCoordsCode)
        {
            var codeBytes = AsmLoader.GetAsmBytes("NoClip_UpdateCoords");
            var zDirection = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            
            AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
            {
                (WorldChrMan.Base.ToInt64(), 0x1 + 2),
                (WorldChrMan.Base.ToInt64(), 0x2A + 2),
                (FieldArea.Base.ToInt64(), 0x6B + 2)
            });
            AsmHelper.WriteRelativeOffsets(codeBytes, new []
            {
                (updateCoordsCode.ToInt64() + 0xBA, zDirection.ToInt64(), 6, 0xBA + 2),
                (updateCoordsCode.ToInt64() + 0xE4, zDirection.ToInt64(), 7, 0xE4 + 2),
                (updateCoordsCode.ToInt64() + 0x102, Hooks.UpdateCoords + 0xB, 5, 0x102 + 1)
            });
            _memoryIo.WriteBytes(updateCoordsCode, codeBytes);
        }

        public void ToggleTargetingView(bool isTargetingViewEnabled) =>
            _memoryIo.WriteByte(TargetView.Base, isTargetingViewEnabled ? 1 : 0);

        public void ToggleReducedTargetingView(bool isTargetingViewEnabled)
        {
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.TargetView.BlueTargetView;
            if (isTargetingViewEnabled)
            {
                var maxDist = CodeCaveOffsets.Base + (int)CodeCaveOffsets.TargetView.MaxDist;
                _memoryIo.WriteFloat(maxDist, 100.0f * 100.0f);
                var codeBytes = AsmLoader.GetAsmBytes("ReduceTargetView");
                var bytes = BitConverter.GetBytes(WorldChrMan.Base.ToInt64());
                var hook = Hooks.BlueTargetView;
                Array.Copy(bytes, 0, codeBytes, 0x36 + 2, 8);
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (code.ToInt64() + 0x86, maxDist.ToInt64(), 8, 0x86 + 4 ),
                    (code.ToInt64() + 0xC4, hook + 0x5, 5, 0xC4 + 1),
                    (code.ToInt64() + 0xCA, hook + 0x141, 5, 0xCA + 1),
                });
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), hook, new byte[]
                    { 0x48, 0x8D, 0x54, 0x24, 0x40 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void SetTargetViewMaxDist(float reducedTargetViewDistance)
        {
            var maxDist = CodeCaveOffsets.Base + (int)CodeCaveOffsets.TargetView.MaxDist;
            _memoryIo.WriteFloat(maxDist, reducedTargetViewDistance * reducedTargetViewDistance);
        }


        public void ForceSave() =>
            _memoryIo.WriteByte((IntPtr)_memoryIo.ReadInt64(GameMan.Base) + GameMan.Offsets.ForceSave, 1);

        public void ToggleDrawHitbox(bool isDrawHitboxEnabled) =>
            _memoryIo.WriteByte((IntPtr)_memoryIo.ReadInt64(DamageManager.Base) + DamageManager.Offsets.HitboxView,
                isDrawHitboxEnabled ? 1 : 0);

        public void ToggleWorldHitDraw(int offset, bool isEnabled) =>
            _memoryIo.WriteByte(WorldHitMan.Base + offset, isEnabled ? 1 : 0);

        public void SetColDrawMode(int val) =>
            _memoryIo.WriteByte(WorldHitMan.Base + WorldHitMan.Offsets.Mode, (byte)val);
    }
}