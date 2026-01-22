// 

namespace TarnishedTool.Models;

public class ParamFieldDef
{
    public string InternalName { get; set; }
    public string DataType { get; set; }
    public int? ArrayLength { get; set; }
    public int? BitPos { get; set; }
    public int? BitWidth { get; set; }
    public string DisplayName { get; set; }
    public int DefaultValue { get; set; }
    public float? Minimum { get; set; }
    public float? Maximum { get; set; }
    public int Offset { get; set; }
}