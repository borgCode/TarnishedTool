using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletFollowType : byte
{
    [Description("Do Not Follow")]
    DoNotFollow = 0,
    [Description("Follow Shooter")]
    FollowShooter = 1,
    [Description("Follow Shooter's Feet")]
    FollowShootersFeet = 2,
    [Description("Follow Target")]
    FollowTarget = 3,
    [Description("Slide Along Ground")]
    SlideAlongGround = 4,
    [Description("Return To Shooter")]
    ReturnToShooter = 5
}