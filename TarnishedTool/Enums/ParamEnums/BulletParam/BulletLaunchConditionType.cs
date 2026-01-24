using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletLaunchConditionType : byte
{
    Always = 0,
    [Description("Hit Water")]
    HitWater = 1,
    [Description("Hit Water Or Swamp")]
    HitWaterOrSwamp = 2,
    [Description("Didn't Hit Enemy")]
    DidntHitEnemy = 3,
    [Description("Hit Enemy")]
    HitEnemy = 4,
    Unknown = 5,
    [Description("Absorbed Bullet")]
    AbsorbedBullet = 6,
    Expired = 254,
    [Description("Hit Ground Or Enemy")]
    HitGroundOrEnemy = 255
}