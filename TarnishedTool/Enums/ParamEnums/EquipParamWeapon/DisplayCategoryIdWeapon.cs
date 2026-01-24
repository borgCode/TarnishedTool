// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum DisplayCategoryIdWeapon : byte
{
    [Description("Dagger - Torch - Shortbow - Arrow")]
    DaggerTorchShortbowArrow = 10,
    [Description("Throwing Blade")]
    ThrowingBlade = 15,
    [Description("Straight Sword - Small Shield - Longbow - Great Arrow")]
    StraightSwordSmallShieldLongbowGreatArrow = 20,
    [Description("Light Greatsword")]
    LightGreatsword = 25,
    [Description("Greatsword - Medium Shield - Greatbow - Bolt")]
    GreatswordMediumShieldGreatbowBolt = 30,
    [Description("Ultra Greatsword - Greatshield - Crossbow - Greatbolt")]
    UltraGreatswordGreatshieldCrossbowGreatbolt = 40,
    [Description("Thrusting Sword - Ballista")]
    ThrustingSwordBallista = 50,
    [Description("Heavy Thrusting Sword - Staff")]
    HeavyThrustingSwordStaff = 60,
    [Description("Curved Sword - Seal")]
    CurvedSwordSeal = 70,
    [Description("Curved Greatsword")]
    CurvedGreatsword = 80,
    [Description("Reverse-hand Blade")]
    ReverseHandBlade = 85,
    Katana = 90,
    [Description("Great Katana")]
    GreatKatana = 95,
    Twinblade = 100,
    Axe = 110,
    Greataxe = 120,
    Hammer = 130,
    Flail = 140,
    [Description("Great Hammer")]
    GreatHammer = 150,
    [Description("Colossal Weapon")]
    ColossalWeapon = 160,
    Spear = 170,
    [Description("Heavy Spear")]
    HeavySpear = 180,
    Halberd = 190,
    Scythe = 200,
    Whip = 210,
    [Description("Fist Weapon")]
    FistWeapon = 220,
    [Description("Hand-to-Hand")]
    HandToHand = 225,
    [Description("Claw Weapon")]
    ClawWeapon = 230,
    [Description("Beast Claw")]
    BeastClaw = 240,
    [Description("Perfume Bottle")]
    PerfumeBottle = 250,
    None = 255
}