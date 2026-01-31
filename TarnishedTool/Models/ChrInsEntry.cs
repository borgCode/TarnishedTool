// 

using System;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Models;

public class ChrInsEntry(nint chrIns) : BaseViewModel
{
    public nint ChrIns { get; set; } = chrIns;
    public int ChrId { get; set; }
    public uint NpcParamId { get; set; }
    public int NpcThinkParamId { get; set; }
    public long Handle { get; set; }
    public string Name { get; set; }
    private Action<ChrInsEntry, string> OnOptionChanged { get; set; }

    public string Display =>
        $@"{Name}   ChrId: {ChrId} NpcParamId: {NpcParamId} NpcThinkParamId: {NpcThinkParamId}";
    
    
    
    
}