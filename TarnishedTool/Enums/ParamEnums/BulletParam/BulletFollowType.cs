namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletFollowType : byte
{
    Do_not_follow = 0,
    Follow_shooter = 1,
    Follow_shooters_feet = 2,
    Follow_target = 3,
    Slide_along_ground = 4,
    Return_to_shooter = 5
}