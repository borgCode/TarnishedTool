// 

namespace TarnishedTool.Models;

public record ParamEntry(uint Id, string Name)
{
    public bool HasName => !string.IsNullOrEmpty(Name);
    public string Name { get; } = Name;
    public uint Id { get; } = Id;
}