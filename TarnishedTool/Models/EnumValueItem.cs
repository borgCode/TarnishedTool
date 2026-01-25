namespace TarnishedTool.Models;

public class EnumValueItem
{
    public string Name { get; set; }
    public object Value { get; set; }
    public string DisplayText => $"{Name} = {Value}";
}