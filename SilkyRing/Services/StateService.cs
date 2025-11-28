using System;
using System.Collections.Generic;
using SilkyRing.Enums;
using SilkyRing.Interfaces;
using SilkyRing.Memory;

namespace SilkyRing.Services;

public class StateService(MemoryService memoryService) : IStateService
{
    
    private readonly Dictionary<State, List<Action>> _eventHandlers = new();
    
    public bool IsLoaded()
    {
        var menuMan = memoryService.ReadInt64(Offsets.MenuMan.Base);
        return memoryService.ReadUInt8((IntPtr)menuMan + Offsets.MenuMan.IsLoaded) == 1;
    }
    
    public void Publish(State eventType)
    {
        if (_eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in _eventHandlers[eventType])
                handler.Invoke();
        }
    }

    public void Subscribe(State eventType, Action handler)
    {
        if (!_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType] = new List<Action>();
   
        _eventHandlers[eventType].Add(handler);
    }

    public void Unsubscribe(State eventType, Action handler)
    {
        if (_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType].Remove(handler);
    }
}