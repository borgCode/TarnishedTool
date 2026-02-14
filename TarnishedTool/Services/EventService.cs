using System;
using System.Linq;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class EventService(IMemoryService memoryService, HookManager hookManager, IReminderService reminderService) : IEventService
    {
        
        public void SetEvent(long eventId, bool flagValue)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.SetEvent);
            AsmHelper.WriteAbsoluteAddresses(bytes, [
                (memoryService.Read<nint>(VirtualMemFlag.Base), 0x4 + 2 ),
                (eventId, 0xE + 2),
                (flagValue ? 1 : 0, 0x18 + 2),
                (Functions.SetEvent, 0x22 + 2)
            ]);
            memoryService.AllocateAndExecute(bytes);
        }

        public bool GetEvent(long flagId)
        {
            var result = CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.GetEvent);
            AsmHelper.WriteAbsoluteAddresses(bytes, [
                (memoryService.Read<nint>(VirtualMemFlag.Base), 0x0 + 2),
                (flagId, 0xA + 2),
                (Functions.GetEvent, 0x18 + 2),
                (result.ToInt64(), 0x28 + 2)
            ]);

            memoryService.AllocateAndExecute(bytes);
            return memoryService.Read<byte>(result) == 1;
        }

        public void PatchEventEnable()
        {
            memoryService.WriteBytes(Patches.CanDrawEvents1, [0xB0, 0x01]);
            memoryService.WriteBytes(Patches.CanDrawEvents2, [0xB0, 0x01]);
        }

        public void ToggleDrawEvents(bool isEnabled)
        {
            var ptr = memoryService.Read<nint>(CSDbgEvent.Base) + CSDbgEvent.DrawEvent;
            memoryService.Write(ptr, isEnabled);
        }

        public void ToggleDisableEvents(bool isEnabled)
        {
            reminderService.TrySetReminder();
            var ptr = memoryService.Read<nint>(CSDbgEvent.Base) + CSDbgEvent.DisableEvent;
            memoryService.Write(ptr, isEnabled);
        }

        public bool AreAllEventsTrue(long[] eventToCheck) => eventToCheck.All(GetEvent);
        
        public void ToggleEvent(long eventId) => SetEvent(eventId, !GetEvent(eventId));
        
        public void ToggleEventLogger(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.EventLogCode;
            if (isEnabled)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.EventLogHook);
                var writeIndex = CodeCaveOffsets.Base + CodeCaveOffsets.EventLogWriteIndex;
                var buffer = CodeCaveOffsets.Base + CodeCaveOffsets.EventLogBuffer;
                var hookLoc = Functions.SetEvent;
                
                AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64() + 0x8, writeIndex.ToInt64(), 6, 0x8 + 2),
                (code.ToInt64() + 0x13, buffer.ToInt64(), 7, 0x13 + 3),
                (code.ToInt64() + 0x2B, writeIndex.ToInt64(), 6, 0x2B + 2),
                (code.ToInt64() + 0x34, hookLoc + 0x5, 5, 0x34 + 1)
                ]);
                
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc, [0x48, 0x89, 0x5C, 0x24, 0x08]);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }
    }
}