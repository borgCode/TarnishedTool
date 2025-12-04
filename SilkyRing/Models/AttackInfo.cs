// 

namespace SilkyRing.Models;

public class AttackInfo
{
    public int MyId { get; set; }
    public float RawPhysicalDamage { get; set; }
    public float RawFireDamage { get; set; }
    public float RawMagicDamage { get; set; }
    public float RawLightningDamage { get; set; }
    public float RawHolyDamage { get; set; }
    public float PoiseDamage { get; set; }
    public PhysicalAttackType PhysicalAttackType { get; set; }
    public int TotalDamage { get; set; }
    public int FireDamage { get; set; }
    public int MagicDamage { get; set; }
    public int LightningDamage { get; set; }
    public int HolyDamage { get; set; }
    public int EnemyId { get; set; }
    
}

public enum PhysicalAttackType
{
    Slash = 0,
    Strike = 1,
    Pierce = 2,
}

