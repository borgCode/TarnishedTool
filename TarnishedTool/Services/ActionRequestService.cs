using System;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class ActionRequestService(IMemoryService memoryService, HookManager hookManager) : IActionRequestService
    {
        private IntPtr _interceptCode;
        private IntPtr _rollFlag;
        private IntPtr _jumpFlag;

        public void ToggleNoRoll(bool enabled)
        {
            if (enabled)
                EnsureHookInstalled();

            memoryService.Write(_rollFlag, enabled ? (byte)1 : (byte)0);

            if (!enabled)
                CheckAndRemoveHookIfNeeded();
        }

        public void ToggleNoJump(bool enabled)
        {
            if (enabled)
                EnsureHookInstalled();

            memoryService.Write(_jumpFlag, enabled ? (byte)1 : (byte)0);

            if (!enabled)
                CheckAndRemoveHookIfNeeded();
        }

        private void EnsureHookInstalled()
        {
            _interceptCode = CodeCaveOffsets.Base + CodeCaveOffsets.ActionRequestIntercept;
            _rollFlag = CodeCaveOffsets.Base + CodeCaveOffsets.NoPlayerRoll;
            _jumpFlag = CodeCaveOffsets.Base + CodeCaveOffsets.NoPlayerJump;

            if (hookManager.IsHookInstalled(_interceptCode.ToInt64()))
                return;

            var bytes = AsmLoader.GetAsmBytes(AsmScript.ActionRequestIntercept);

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (_interceptCode.ToInt64() + 0x5, _rollFlag.ToInt64(), 7, 0x7),
                (_interceptCode.ToInt64() + 0x15, _jumpFlag.ToInt64(), 7, 0x17),
            });

            memoryService.WriteBytes(_interceptCode, bytes);
            memoryService.Write(_rollFlag, (byte)0);
            memoryService.Write(_jumpFlag, (byte)0);

            hookManager.InstallHook(
                _interceptCode.ToInt64(),
                Hooks.SetActionRequested,
                [0x49, 0x09, 0x41, 0x10, 0xC3]);
        }

        private void CheckAndRemoveHookIfNeeded()
        {
            if (memoryService.Read<byte>(_rollFlag) == 0 &&
                memoryService.Read<byte>(_jumpFlag) == 0)
            {
                hookManager.UninstallHook(_interceptCode.ToInt64());
            }
        }
    }
}
