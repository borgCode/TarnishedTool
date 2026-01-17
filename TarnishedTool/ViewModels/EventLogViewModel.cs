// 

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TarnishedTool.ViewModels;

public class EventLogViewModel
{
    private ObservableCollection<string> _logEntries = new();
    public ObservableCollection<string> LogEntries => _logEntries;

    public void RefreshEventLogs(List<(uint eventId, bool value)> entries)
    {
        foreach (var (eventId, val) in entries)
        {
            _logEntries.Add($"Event {eventId}: {(val ? "TRUE" : "FALSE")}");
        }

        while (_logEntries.Count > 2000)
            _logEntries.RemoveAt(0);
    }

    public void Clear() => _logEntries.Clear();
}