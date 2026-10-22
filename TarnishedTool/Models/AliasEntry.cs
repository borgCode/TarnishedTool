using System.Collections.Generic;

namespace TarnishedTool.Models;

public class AliasEntry
{
    public List<string> Names { get; set; } = new(); 
    public List<string> Targets { get; set; } = new();
}