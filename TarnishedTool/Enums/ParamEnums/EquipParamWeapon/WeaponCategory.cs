// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum WeaponCategory : byte
{
    Default = 0,
    Unknown1 = 1,
    [Description("Thrusting Sword")]
    ThrustingSword = 2,
    Sword = 3,
    Axe = 4,
    Hammer = 5,
    Unknown6 = 6,
    Unknown7 = 7,
    Staff = 8,
    Fist = 9,
    Bow = 10,
    Crossbow = 11,
    Torch = 12,
    Unknown13 = 13,
    Unknown14 = 14
}