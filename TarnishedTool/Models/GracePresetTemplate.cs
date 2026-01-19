// 

using System.Collections.Generic;

namespace TarnishedTool.Models;

public class GracePresetTemplate
{
    public string Name { get; set; }
    public List<GracePresetEntry> Graces { get; set; } = new();
}