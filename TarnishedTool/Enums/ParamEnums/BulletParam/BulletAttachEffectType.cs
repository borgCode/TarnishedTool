using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletAttachEffectType : byte
{
    None = 0,
    Force = 1,
    [Description("Unknown Protection")]
    UnknownProtection = 2,
    [Description("Dark Force")]
    DarkForce = 3,
    Unknown = 4,
    [Description("Absorb Bullet")]
    AbsorbBullet = 5,
    [Description("Trigger Remote Detonation")]
    TriggerRemoteDetonation = 6
}