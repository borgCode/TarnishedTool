using System;
using System.Collections.Generic;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Memory;

namespace TarnishedTool.Services;

public class StateService(IMemoryService memoryService) : IStateService
{
    private readonly Dictionary<State, List<Action>> _eventHandlers = new();

    public bool IsLoaded()
    {
        var worldChrman = memoryService.ReadInt64(Offsets.WorldChrMan.Base);
        return memoryService.ReadInt64((IntPtr)worldChrman + Offsets.WorldChrMan.PlayerIns) != 0;
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