// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum WepMotionCategory : byte
{
    None = 0,
    Dagger = 20,
    Torch = 21,
    Claw = 22,
    [Description("Straight Sword")]
    StraightSword = 23,
    Twinblade = 24,
    Greatsword = 25,
    [Description("Colossal Sword")]
    ColossalSword = 26,
    [Description("Thrusting Sword")]
    ThrustingSword = 27,
    [Description("Curved Sword")]
    CurvedSword = 28,
    Katana = 29,
    Axe = 30,
    [Description("Colossal Weapon")]
    ColossalWeapon = 31,
    Greataxe = 32,
    Hammer = 33,
    Flail = 34,
    [Description("Great Hammer")]
    GreatHammer = 35,
    Spear = 36,
    [Description("Great Spear")]
    GreatSpear = 37,
    Halberd = 38,
    [Description("Heavy Thrusting Sword")]
    HeavyThrustingSword = 39,
    [Description("Curved Greatsword")]
    CurvedGreatsword = 40,
    Catalyst = 41,
    Fist = 42,
    Whip = 43,
    Bow = 44,
    Greatbow = 45,
    Crossbow = 46,
    Greatshield = 47,
    [Description("Small Shield")]
    SmallShield = 48,
    [Description("Medium Shield")]
    MediumShield = 49,
    Scythe = 50,
    [Description("Light Bow")]
    LightBow = 51,
    Ballista = 52,
    [Description("Throwing Blade")]
    ThrowingBlade = 53,
    [Description("Hand-to-Hand")]
    HandToHand = 55,
    [Description("Perfume Bottle")]
    PerfumeBottle = 56,
    [Description("Thrusting Shield")]
    ThrustingShield = 57,
    [Description("Reverse-hand Sword")]
    ReverseHandSword = 58,
    [Description("Light Greatsword")]
    LightGreatsword = 60,
    [Description("Great Katana")]
    GreatKatana = 61,
    [Description("Beast Claw")]
    BeastClaw = 62
}