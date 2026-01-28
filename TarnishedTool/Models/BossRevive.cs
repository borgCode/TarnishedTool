// 

using System.Collections.Generic;

namespace TarnishedTool.Models;

public class BossRevive
{
    public bool IsDlc { get; set; }
    public string Area  { get; set; }
    public string BossName { get; set; }
    public bool IsInitializeDeadSet { get; set; }
    public List<uint> NpcParamIds { get; set; }
    public uint BlockId { get; set; }
    public List<BossFlag> FirstEncounterFlags { get; set; }
    public List<BossFlag> BossFlags { get; set; }
    public Position Position { get; set; }
    public Position PositionFirstEncounter { get; set; }
    public bool ShouldSetNight { get; set; }
    public List<uint> BossBlockIds { get; set; }
}