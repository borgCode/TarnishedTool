// 

using System;
using System.Collections.Generic;
using TarnishedTool.Models;

namespace TarnishedTool.Interfaces;

public interface IEventLogReader
{
    event Action<List<EventLogEntry>> EntriesReceived;
    void Start();
    void Stop();
}