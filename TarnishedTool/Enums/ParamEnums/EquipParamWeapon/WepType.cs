// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum WepType : ushort
{
    None = 0,
    Dagger = 1,
    [Description("Straight Sword")]
    StraightSword = 3,
    Greatsword = 5,
    [Description("Colossal Sword")]
    ColossalSword = 7,
    [Description("Curved Sword")]
    CurvedSword = 9,
    [Description("Curved Greatsword")]
    CurvedGreatsword = 11,
    Katana = 13,
    Twinblade = 14,
    [Description("Thrusting Sword")]
    ThrustingSword = 15,
    [Description("Heavy Thrusting Sword")]
    HeavyThrustingSword = 16,
    Axe = 17,
    Greataxe = 19,
    Hammer = 21,
    [Description("Great Hammer")]
    GreatHammer = 23,
    Flail = 24,
    Spear = 25,
    [Description("Heavy Spear")]
    HeavySpear = 28,
    Halberd = 29,
    Scythe = 31,
    Fist = 35,
    Claw = 37,
    Whip = 39,
    [Description("Colossal Weapon")]
    ColossalWeapon = 41,
    [Description("Light Bow")]
    LightBow = 50,
    Bow = 51,
    Greatbow = 53,
    Crossbow = 55,
    Ballista = 56,
    Staff = 57,
    Seal = 61,
    [Description("Small Shield")]
    SmallShield = 65,
    [Description("Medium Shield")]
    MediumShield = 67,
    Greatshield = 69,
    Arrow = 81,
    Greatarrow = 83,
    Bolt = 85,
    [Description("Ballista Bolt")]
    BallistaBolt = 86,
    Torch = 87,
    [Description("Hand-to-Hand")]
    HandToHand = 88,
    [Description("Perfume Bottle")]
    PerfumeBottle = 89,
    [Description("Thrusting Shield")]
    ThrustingShield = 90,
    [Description("Throwing Blade")]
    ThrowingBlade = 91,
    [Description("Reverse-hand Blade")]
    ReverseHandBlade = 92,
    [Description("Light Greatsword")]
    LightGreatsword = 93,
    [Description("Great Katana")]
    GreatKatana = 94,
    [Description("Beast Claw")]
    BeastClaw = 95
}