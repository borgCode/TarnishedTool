using System;
using System.Threading;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Models;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class TravelService(IMemoryService memoryService, HookManager hookManager) : ITravelService
    {
        public void Warp(Grace grace)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.GraceWarp);
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (WorldChrMan.Base.ToInt64(), 0x0 + 2),
                (grace.GraceEntityId, 0x12 + 2),
                (Functions.GraceWarp, 0x20 + 2)
            });

            memoryService.AllocateAndExecute(bytes);
        }

        public void WarpToBlockId(Position position)
        {
            int area = (int)(position.BlockId >> 24) & 0xFF;
            int block = (int)(position.BlockId >> 16) & 0xFF;
            int map = (int)(position.BlockId >> 8) & 0xFF;
            int altNo = (int)position.BlockId & 0xFF;

            var bytes = AsmLoader.GetAsmBytes(AsmScript.WarpToBlock);
            AsmHelper.WriteAbsoluteAddress(bytes, Functions.WarpToBlock, 0x16 + 2);
            AsmHelper.WriteImmediateDwords(bytes, new[]
            {
                (area, 0x0 + 1),
                (block, 0x5 + 1),
                (map, 0xA + 2),
                (altNo, 0x10 + 2),
            });

            memoryService.AllocateAndExecute(bytes);

            HookWarpCoordWrites(position);
        }

        public void ToggleShowAllGraces(bool isEnabled) =>
            memoryService.Write(MapDebugFlags.Base + MapDebugFlags.ShowAllGraces, isEnabled);

        public void ToggleShowAllMaps(bool isEnabled) =>
            memoryService.Write(MapDebugFlags.Base + MapDebugFlags.ShowAllMaps, isEnabled);

        public void ToggleNoMapAcquiredPopups(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoAcquiredMapPopup;
            if (isEnabled)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.NoAcquiredMapPopup);
                var hookLoc = Hooks.NoMapAcquiredPopup;
                var skipDialogCreationJumpTarget = hookLoc + 0xD;
                AsmHelper.WriteRelativeOffsets(bytes, new[]
                {
                    (code.ToInt64() + 0x7, VirtualMemFlag.Base.ToInt64(), 7, 0x7 + 3),
                    (code.ToInt64() + 0x20, Functions.SetEvent, 5, 0x20 + 1),
                    (code.ToInt64() + 0x30, skipDialogCreationJumpTarget, 5, 0x30 + 1)
                });
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc, [0x8B, 0x54, 0x24, 0x40, 0x48, 0x8B, 0xCE]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        private void HookWarpCoordWrites(Position position)
        {
            int angleOffsetInStruct = 0xAB0;

            var coordHook = Hooks.WarpCoordWrite;
            var angleHook = Hooks.WarpAngleWrite;

            var targetCoords = CodeCaveOffsets.Base + CodeCaveOffsets.WarpCoords;
            var targetAngle = CodeCaveOffsets.Base + CodeCaveOffsets.Angle;
            var warpCode = CodeCaveOffsets.Base + CodeCaveOffsets.WarpCode;
            var angleCode = CodeCaveOffsets.Base + CodeCaveOffsets.AngleCode;
            memoryService.Write(targetCoords, position.Coords);
            memoryService.Write(targetCoords + 0xC, 1f);
            memoryService.Write(targetAngle + 0x4, position.Angle);

            var bytes = AsmLoader.GetAsmBytes(AsmScript.WarpCoordWrite);
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (warpCode.ToInt64(), targetCoords.ToInt64(), 7, 0x0 + 3),
                (warpCode.ToInt64() + 0xE, coordHook + 7, 5, 0xE + 1)
            });
            memoryService.WriteBytes(warpCode, bytes);

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (angleCode.ToInt64(), targetAngle.ToInt64(), 7, 0x0 + 3),
                (angleCode.ToInt64() + 0xE, angleHook + 7, 5, 0xE + 1)
            });
            memoryService.WriteBytes(angleCode, bytes);
            memoryService.Write(angleCode + 0x7 + 3, angleOffsetInStruct);

            hookManager.InstallHook(warpCode.ToInt64(), coordHook, [0x0F, 0x11, 0x80, 0xA0, 0x0A, 0x00, 0x00]);
            hookManager.InstallHook(angleCode.ToInt64(), angleHook, [0x0F, 0x11, 0x80, 0xB0, 0x0A, 0x00, 0x00]);

            var isFadedPtr = memoryService.Read<nint>(MenuMan.Base) + MenuMan.IsFading;
            var fadeBit = (byte)MenuMan.FadeBitFlags.IsFadeScreen;

            WaitForCondition(() => memoryService.IsBitSet(isFadedPtr, fadeBit));
            WaitForCondition(() => !memoryService.IsBitSet(isFadedPtr, fadeBit));

            hookManager.UninstallHook(warpCode.ToInt64());
            hookManager.UninstallHook(angleCode.ToInt64());
        }

        private void WaitForCondition(Func<bool> condition, int timeoutMs = 10000, int pollMs = 50)
        {
            int start = Environment.TickCount;
            while (!condition() && Environment.TickCount < start + timeoutMs)
            {
                Thread.Sleep(pollMs);
            }
        }
    }
}