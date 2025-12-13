using SilkyRing.Interfaces;
using SilkyRing.Memory;
using SilkyRing.Models;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class TravelService(MemoryService memoryService, HookManager hookManager) : ITravelService
    {

        public void Warp(Grace grace)
        {
            var bytes = AsmLoader.GetAsmBytes("GraceWarp");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (WorldChrMan.Base.ToInt64(), 0x0 + 2),
                (grace.GraceEntityId, 0x12 + 2),
                (Functions.GraceWarp, 0x20 + 2)
            });
            
            memoryService.AllocateAndExecute(bytes);
        }

        public void UnlockGrace(Grace grace)
        {
            var bytes = AsmLoader.GetAsmBytes("SetEvent");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (memoryService.ReadInt64(VirtualMemFlag.Base), 0x4 + 2 ),
                (grace.FlagId, 0xE + 2),
                (1, 0x18 + 2),
                (Functions.SetEvent, 0x22 + 2)
            });
            memoryService.AllocateAndExecute(bytes);
        }
        
    }
}