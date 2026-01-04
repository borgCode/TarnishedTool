using System;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;
using TarnishedTool.Utilities;
using static TarnishedTool.Memory.Offsets;

namespace TarnishedTool.Services
{
    public class EventService(MemoryService memoryService, HookManager hookManager, IReminderService reminderService) : IEventService
    {
        
        public void SetEvent(long eventId, bool flagValue)
        {
            var bytes = AsmLoader.GetAsmBytes("SetEvent");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (memoryService.ReadInt64(VirtualMemFlag.Base), 0x4 + 2 ),
                (eventId, 0xE + 2),
                (flagValue ? 1 : 0, 0x18 + 2),
                (Functions.SetEvent, 0x22 + 2)
            });
            memoryService.AllocateAndExecute(bytes);
        }

        public bool GetEvent(long flagId)
        {
            var result = CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult;
            var bytes = AsmLoader.GetAsmBytes("GetEvent");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (memoryService.ReadInt64(VirtualMemFlag.Base), 0x0 + 2),
                (flagId, 0xA + 2),
                (Functions.GetEvent, 0x18 + 2),
                (result.ToInt64(), 0x28 + 2)
            });

            memoryService.AllocateAndExecute(bytes);
            return memoryService.ReadUInt8(result) == 1;
        }

        public void PatchEventEnable()
        {
            memoryService.WriteBytes(Patches.CanDrawEvents1, [0xB0, 0x01]);
            memoryService.WriteBytes(Patches.CanDrawEvents2, [0xB0, 0x01]);
        }

        public void ToggleDrawEvents(bool isEnabled)
        {
            var ptr = memoryService.ReadInt64(CSDbgEvent.Base) + CSDbgEvent.DrawEvent;
            memoryService.WriteUInt8((IntPtr)ptr, isEnabled ? 1 : 0);
        }

        public void ToggleDisableEvents(bool isEnabled)
        {
            reminderService.TrySetReminder();
            var ptr = memoryService.ReadInt64(CSDbgEvent.Base) + CSDbgEvent.DisableEvent;
            memoryService.WriteUInt8((IntPtr)ptr, isEnabled ? 1 : 0);
        }
    }
}