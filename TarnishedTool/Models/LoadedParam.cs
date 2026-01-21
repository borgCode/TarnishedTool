// 

using System.Collections.Generic;

namespace TarnishedTool.Models;

public class LoadedParam
{
    public string Name { get; set; }
    public int TableIndex { get; set; }
    public int SlotIndex { get; set; }
    public int RowSize { get; set; } 
    public IReadOnlyList<ParamFieldDef> Fields { get; set; }
    public IReadOnlyList<ParamEntry> Entries { get; set; }
}
