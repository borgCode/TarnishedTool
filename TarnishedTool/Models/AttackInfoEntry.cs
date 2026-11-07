// 

using System;
using System.Collections.Generic;

namespace TarnishedTool.Models;

public class AttackInfoEntry
{
    public int Id { get; }
    public int EnemyId { get; }
    public string AttackType { get; }
    
    public int TotalDamage { get; }
    public int PhysicalDamage { get; }
    public int FireDamage { get; }
    public int MagicDamage { get; }
    public int LightningDamage { get; }
    public int HolyDamage { get; }
    
    public float RawPhysicalDamage { get; }
    public float RawFireDamage { get; }
    public float RawMagicDamage { get; }
    public float RawLightningDamage { get; }
    public float RawHolyDamage { get; }
    
    public float PoiseDamage { get; }
    
    public string RawSplitText { get; }
    public string FinalSplitText { get; }
    
    public bool HasRawSplit => !string.IsNullOrEmpty(RawSplitText);
    public bool HasFinalSplit => !string.IsNullOrEmpty(FinalSplitText);
    
    public bool HasFireDamage => FireDamage > 0;
    public bool HasMagicDamage => MagicDamage > 0;
    public bool HasLightningDamage => LightningDamage > 0;
    public bool HasHolyDamage => HolyDamage > 0;
    
    public bool HasRawFireDamage => RawFireDamage > 0;
    public bool HasRawMagicDamage => RawMagicDamage > 0;
    public bool HasRawLightningDamage => RawLightningDamage > 0;
    public bool HasRawHolyDamage => RawHolyDamage > 0;

    public AttackInfoEntry(AttackInfo attack)
    {
        Id = attack.MyId;
        EnemyId = attack.EnemyId;
        
        AttackType = attack.PhysicalAttackType switch
        {
            PhysicalAttackType.Slash => "Slash",
            PhysicalAttackType.Strike => "Strike",
            PhysicalAttackType.Pierce => "Pierce",
            PhysicalAttackType.Standard => "Standard",
            _ => "Unknown"
        };
        
        TotalDamage = attack.TotalDamage;
        FireDamage = attack.FireDamage;
        MagicDamage = attack.MagicDamage;
        LightningDamage = attack.LightningDamage;
        HolyDamage = attack.HolyDamage;
        
        PhysicalDamage = TotalDamage - FireDamage - MagicDamage - LightningDamage - HolyDamage;
        if (PhysicalDamage < 0) PhysicalDamage = 0;
        
        RawPhysicalDamage = attack.RawPhysicalDamage;
        RawFireDamage = attack.RawFireDamage;
        RawMagicDamage = attack.RawMagicDamage;
        RawLightningDamage = attack.RawLightningDamage;
        RawHolyDamage = attack.RawHolyDamage;
        
        PoiseDamage = attack.PoiseDamage;
        
        RawSplitText = CalculateSplit(
            ("Phys", RawPhysicalDamage),
            ("Magic", RawMagicDamage),
            ("Fire", RawFireDamage),
            ("Ltn", RawLightningDamage),
            ("Holy", RawHolyDamage)
        );

        FinalSplitText = CalculateSplit(
            ("Phys", PhysicalDamage),
            ("Magic", MagicDamage),
            ("Fire", FireDamage),
            ("Ltn", LightningDamage),
            ("Holy", HolyDamage)
        );
    }

    private static string CalculateSplit(params (string Name, float Value)[] types)
    {
        var active = new List<(string Name, float Value)>();
        
        foreach (var t in types)
        {
            if (t.Value > 0)
                active.Add(t);
        }

        if (active.Count != 2)
            return string.Empty;

        float total = active[0].Value + active[1].Value;
        if (total <= 0)
            return string.Empty;

        double pct1 = Math.Round(active[0].Value / total * 100, 1);
        double pct2 = Math.Round(active[1].Value / total * 100, 1);

        return $"{active[0].Name}/{active[1].Name} {pct1}%/{pct2}%";
    }
}