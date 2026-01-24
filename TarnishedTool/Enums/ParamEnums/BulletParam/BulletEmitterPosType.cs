using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletEmitterPosType : byte
{
    Default = 0,
    [Description("Shooter And Random Create Radius")]
    ShooterAndRandomCreateRadius = 1,
    [Description("Higher Elevation")]
    HigherElevation = 2,
    [Description("Nearby Enemy Origin (If No Enemies, Targets Bullet Origin)")]
    NearbyEnemyOriginIfNoEnemiesTargetsBulletOrigin = 3,
    [Description("Nearest Enemy And Random Create Radius")]
    NearestEnemyAndRandomCreateRadius = 4,
    [Description("Parent Bullet Instead Of Hit Location")]
    ParentBulletInsteadOfHitLocation = 5,
    [Description("Above And Behind Target")]
    AboveAndBehindTarget = 6

}