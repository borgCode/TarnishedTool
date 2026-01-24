// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum GuardMotionCategory : byte
{
    [Description("Medium Shield")]
    MediumShield = 0,
    Greatshield = 1,
    [Description("Small Shield")]
    SmallShield = 2
}