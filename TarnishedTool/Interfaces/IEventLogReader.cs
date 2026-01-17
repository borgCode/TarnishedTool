// 

using System;
using System.Collections.Generic;

namespace TarnishedTool.Interfaces;

public interface IEventLogReader
{
    event Action<List<(uint eventId, bool value)>> EntriesReceived;
    void Start();
    void Stop();
}