// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamGoods;

public enum GoodsUselimitSummonbuddy : byte
{
    None = 0,
    [Description("Apply First [10000]")]
    ApplyFirst10000 = 1,
    [Description("Apply First (Damage Level Change)")]
    ApplyFirstDamageLevelChange = 2,
    [Description("Persist through Death")]
    PersistThroughDeath = 3,
    [Description("Apply First [10002]")]
    ApplyFirst10002 = 4,
    [Description("Apply Highest (Category Priority) [1000]")]
    ApplyHighestCategoryPriority1000 = 5,
    [Description("Apply Highest (Category Priority) [1001]")]
    ApplyHighestCategoryPriority1001 = 6,
    [Description("Apply First (Blood Loss)")]
    ApplyFirstBloodLoss = 7,
    [Description("Apply First (Poison)")]
    ApplyFirstPoison = 8,
    [Description("Apply First (Scarlet Rot)")]
    ApplyFirstScarletRot = 9,
    [Description("Apply First (Frostbite)")]
    ApplyFirstFrostbite = 10,
    [Description("Apply First (Damage Level Change) + Persist through Death")]
    ApplyFirstDamageLevelChangePersistThroughDeath = 11,
    [Description("Apply First (Damage Level Change) + Persist through Death + Apply First [10002]")]
    ApplyFirstDamageLevelChangePersistThroughDeathApplyFirst10002 = 12,
    [Description("Apply First (Damage Level Change) + Apply First [10002]")]
    ApplyFirstDamageLevelChangeApplyFirst10002 = 13,
    [Description("Apply First [10006]")]
    ApplyFirst10006 = 14,
    [Description("Apply First (Death)")]
    ApplyFirstDeath = 15,
    [Description("Apply First (Sleep)")]
    ApplyFirstSleep = 16,
    [Description("Apply First (Madness)")]
    ApplyFirstMadness = 17
}