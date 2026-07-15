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
        private nint _interceptCode;
        private nint _rollFlag;
        private nint _jumpFlag;

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

            if (hookManager.IsHookInstalled(_interceptCode))
                return;

            var bytes = AsmLoader.GetAsmBytes(AsmScript.ActionRequestIntercept);

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (_interceptCode + 0x5, _rollFlag, 7, 0x7),
                (_interceptCode + 0x15, _jumpFlag, 7, 0x17),
            });

            memoryService.WriteBytes(_interceptCode, bytes);
            memoryService.Write(_rollFlag, (byte)0);
            memoryService.Write(_jumpFlag, (byte)0);

            hookManager.InstallHook(
                _interceptCode,
                Hooks.SetActionRequested,
                [0x49, 0x09, 0x41, 0x10, 0xC3]);
        }

        private void CheckAndRemoveHookIfNeeded()
        {
            if (memoryService.Read<byte>(_rollFlag) == 0 &&
                memoryService.Read<byte>(_jumpFlag) == 0)
            {
                hookManager.UninstallHook(_interceptCode);
            }
        }
    }
}
