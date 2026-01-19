// 

namespace TarnishedTool.Models;

public class Weather(sbyte type, string name)
{
    public sbyte Type { get; set; } = type;
    public string Name { get; set; } = name;
}