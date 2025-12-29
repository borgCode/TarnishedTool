// 

using System.Collections.Generic;

namespace TarnishedTool.Models;

public class BossRevive
{
    public bool IsDlc { get; set; }
    public string BossName { get; set; }
    public bool IsInitializeDeadSet { get; set; }
    public List<BossFlag> FirstEncounterFlags { get; set; }
    public List<BossFlag> BossFlags { get; set; }
}