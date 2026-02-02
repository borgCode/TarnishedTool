// 

namespace TarnishedTool.Models;


public enum ParamType {Float, Int, UInt, Bool, Enum}

public class GoalParamDef(string name, ParamType paramType, string enumDictName)
{
    public string Name { get; set; } = name;
    public ParamType ParamType { get; } = paramType;
    public string EnumDictName { get; set; } = enumDictName;
}