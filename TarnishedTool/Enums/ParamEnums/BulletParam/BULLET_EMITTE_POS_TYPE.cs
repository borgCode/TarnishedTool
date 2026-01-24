namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BULLET_EMITTE_POS_TYPE : byte
{
    Default = 0,
    ShooterAndRandomCreateRadius = 1,
    HigherElevation = 2,
    NearbyEnemyOrigin_IfNoEnemies_TargetsBulletOrigin = 3,
    NearestEnemy_and_randomCreateRadius = 4,
    ParentBullet_InsteadOfHitLocation = 5,
    AboveAndBehindTarget = 6

}