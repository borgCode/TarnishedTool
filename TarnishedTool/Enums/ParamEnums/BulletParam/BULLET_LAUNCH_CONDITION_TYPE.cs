namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BULLET_LAUNCH_CONDITION_TYPE : byte
{
    Always = 0,
    Hit_water = 1,
    Hit_water_or_swamp = 2,
    Didnt_hit_enemy = 3,
    Hit_enemy = 4,
    Unknown = 5,
    Absorbed_Bullet = 6,
    Expired = 254,
    Hit_ground_or_enemy = 255

}