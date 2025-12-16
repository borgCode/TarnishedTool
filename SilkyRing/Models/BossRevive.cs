// 

using System.Collections.Generic;

namespace SilkyRing.Models;

public class BossRevive
{
    public bool IsDlc { get; set; }
    public string BossName { get; set; }
    public bool IsInitializeDeadSet { get; set; }
    public List<BossFlag> BossFlags { get; set; }
}