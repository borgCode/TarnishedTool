using System;
using SilkyRing.Memory;
using SilkyRing.Models;
using SilkyRing.Utilities;
using static SilkyRing.Memory.Offsets;

namespace SilkyRing.Services
{
    public class TravelService
    {
        
        private readonly MemoryService _memoryService;
        private readonly HookManager _hookManager;
        public TravelService(MemoryService memoryService, HookManager hookManager)
        {
            _memoryService = memoryService;
            _hookManager = hookManager;
        }


        public void Warp(Grace grace)
        {
            var bytes = AsmLoader.GetAsmBytes("GraceWarp");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (WorldChrMan.Base.ToInt64(), 0x0 + 2),
                (grace.GraceEntityId, 0x12 + 2),
                (Funcs.GraceWarp, 0x20 + 2)
            });
            
            _memoryService.AllocateAndExecute(bytes);
        }

        public void UnlockGrace(Grace grace)
        {
            var bytes = AsmLoader.GetAsmBytes("SetEvent");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (_memoryService.ReadInt64(VirtualMemFlag.Base), 0x4 + 2 ),
                (grace.FlagId, 0xE + 2),
                (1, 0x18 + 2),
                (Funcs.SetEvent, 0x22 + 2)
            });
            _memoryService.AllocateAndExecute(bytes);
        }

        public void Test()
        {
            
        }
    }
}