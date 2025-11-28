using System;
using SilkyRing.Memory;
using SilkyRing.Utilities;

namespace SilkyRing.Services
{
    public class EventService
    {
        private readonly MemoryService _memoryService;
        private readonly HookManager _hookManager;
        
        public EventService(MemoryService memoryService, HookManager hookManager)
        {
            _memoryService = memoryService;
            _hookManager = hookManager;
        }
        
    }
}