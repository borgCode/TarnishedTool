// 

namespace TarnishedTool.Models;

public class EventLogEntry(uint eventId, bool value)
{
    public uint EventId { get; } = eventId;
    public bool Value { get; } = value;
    public string DisplayText => $"Event {EventId}: {(Value ? "TRUE" : "FALSE")}";

    public override string ToString() => DisplayText;

}