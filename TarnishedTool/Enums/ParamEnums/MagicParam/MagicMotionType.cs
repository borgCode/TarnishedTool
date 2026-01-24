// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.MagicParam;

public enum MagicMotionType : byte
{
    [Description("Head Clutch")]
    HeadClutch = 0,
    [Description("Raise Front, then Quick Cast")]
    RaiseFrontThenQuickCast = 1,
    [Description("Raise Front, then Cast")]
    RaiseFrontThenCast = 2,
    [Description("Raise Side, then Cast")]
    RaiseSideThenCast = 4,
    [Description("Magic Bow Cast")]
    MagicBowCast = 5,
    [Description("Float, then Curl Up")]
    FloatThenCurlUp = 6,
    [Description("Spell Stream")]
    SpellStream = 7,
    [Description("Raise Up, then Cast")]
    RaiseUpThenCast = 8,
    [Description("Raise Side and Quick Cast")]
    RaiseSideAndQuickCast = 9,
    [Description("Hold, then Thrust")]
    HoldThenThrust = 10,
    [Description("Enchant - Weapon")]
    EnchantWeapon = 11,
    [Description("Enchant - Shield")]
    EnchantShield = 12,
    [Description("Enchant - Raise Weapon")]
    EnchantRaiseWeapon = 13,
    [Description("Raise Arms and Slam")]
    RaiseArmsAndSlam = 14,
    [Description("Slam, then Hold")]
    SlamThenHold = 15,
    [Description("Side Swish")]
    SideSwish = 16,
    [Description("Raise, Charge, then Grab")]
    RaiseChargeThenGrab = 17,
    [Description("Hold Sidewards, then Release")]
    HoldSidewardsThenRelease = 19,
    [Description("Frontal Swish")]
    FrontalSwish = 20,
    [Description("Raise Arms, then Frontal Swing")]
    RaiseArmsThenFrontalSwing = 22,
    [Description("Emit from Hand")]
    EmitFromHand = 23,
    [Description("Lift Pose, then Wide Slam")]
    LiftPoseThenWideSlam = 24,
    [Description("Hold, then Hand Flick")]
    HoldThenHandFlick = 25,
    [Description("Raise Front, then Smooth Cast")]
    RaiseFrontThenSmoothCast = 26,
    [Description("Raise Above, then Fancy Slash")]
    RaiseAboveThenFancySlash = 27,
    [Description("Cross Arms, then Explode")]
    CrossArmsThenExplode = 28,
    [Description("Cross Arms, then Float and Explode")]
    CrossArmsThenFloatAndExplode = 29,
    [Description("Side Hold, then Slam into Chest")]
    SideHoldThenSlamIntoChest = 31,
    [Description("Quick Enchant")]
    QuickEnchant = 32,
    [Description("Hold Head")]
    HoldHead = 33,
    [Description("Kneel, Hold Head")]
    KneelHoldHead = 34,
    [Description("Kneel, Hold Head, then Stand and Raise Hand")]
    KneelHoldHeadThenStandAndRaiseHand = 35,
    [Description("Swing Back, then Focused Slash")]
    SwingBackThenFocusedSlash = 36,
    [Description("Swing Back and Hunch, then Heavy Slash")]
    SwingBackAndHunchThenHeavySlash = 37,
    [Description("Frontal Low Slash")]
    FrontalLowSlash = 38,
    [Description("Hand to Ground, then Swipe Up")]
    HandToGroundThenSwipeUp = 40,
    [Description("Swing Back High, then Erupt Ground")]
    SwingBackHighThenEruptGround = 41,
    [Description("Focus Front, then Raise Hand Up")]
    FocusFrontThenRaiseHandUp = 42,
    [Description("Swing Back, then Jump and Slash")]
    SwingBackThenJumpAndSlash = 44,
    [Description("Swing Back, then Jump and Slash (Lightning)")]
    SwingBackThenJumpAndSlashLightning = 45,
    [Description("Raise Hands, then Lower and Emit")]
    RaiseHandsThenLowerAndEmit = 48,
    [Description("Golden Order Stance, Right Arm Out")]
    GoldenOrderStanceRightArmOut = 49,
    [Description("Dragon Breath, Emit Forward")]
    DragonBreathEmitForward = 50,
    [Description("Dragon Breath, Emit Sweeping")]
    DragonBreathEmitSweeping = 51,
    [Description("Float, then Dragon Claw Slam")]
    FloatThenDragonClawSlam = 52,
    [Description("Float, then Dragon Slam")]
    FloatThenDragonSlam = 54,
    [Description("Dragon Roar")]
    DragonRoar = 55,
    [Description("Hold, then Multiple Slams")]
    HoldThenMultipleSlams = 56,
    [Description("Hold, Charge, then Stomp")]
    HoldChargeThenStomp = 57,
    [Description("Hold, then Body Emit")]
    HoldThenBodyEmit = 58,
    [Description("Stance, then Raise Hand (Lightning)")]
    StanceThenRaiseHandLightning = 59,
    [Description("Raise Arms Crossed, then Step Release")]
    RaiseArmsCrossedThenStepRelease = 60,
    [Description("Emit Frenzy from Eyes")]
    EmitFrenzyFromEyes = 66,
    [Description("Lesser Dragon Breath")]
    LesserDragonBreath = 67,
    [Description("Step Forward, then Spin Tail")]
    StepForwardThenSpinTail = 70,
    [Description("Low Stance, then Quick Shield Bash")]
    LowStanceThenQuickShieldBash = 71,
    [Description("Low Hold, then Raise and Release")]
    LowHoldThenRaiseAndRelease = 74,
    [Description("Side Hold, then Smooth Release")]
    SideHoldThenSmoothRelease = 75,
    [Description("Swing Back, then Slow Forward Release")]
    SwingBackThenSlowForwardRelease = 76,
    [Description("Swing Back, then Slam Hammer")]
    SwingBackThenSlamHammer = 78,
    [Description("Forward Hold, then Quick Cast")]
    ForwardHoldThenQuickCast = 79,
    [Description("Side Hold, then Quick Slash")]
    SideHoldThenQuickSlash = 80,
    [Description("Raise Above, then Erupt Chest")]
    RaiseAboveThenEruptChest = 81,
    [Description("Raise Above, then Front Slam")]
    RaiseAboveThenFrontSlam = 82,
    [Description("Raise Forward, then Back Swing")]
    RaiseForwardThenBackSwing = 83,
    [Description("Carian Retaliation")]
    CarianRetaliation = 84,
    [Description("Golden Order Stance: Balance Scales Motion")]
    GoldenOrderStanceBalanceScalesMotion = 86,
    [Description("Raise Arms Crossed in Front, then Emit")]
    RaiseArmsCrossedInFrontThenEmit = 87,
    [Description("Clutch Head, then Emit Frenzy")]
    ClutchHeadThenEmitFrenzy = 88,
    [Description("Clutch Head, then Explode Frenzy")]
    ClutchHeadThenExplodeFrenzy = 89,
    [Description("Clutch Head, then Writhe")]
    ClutchHeadThenWrithe = 90,
    [Description("Wide Arms, then Crouch and Emit Upwards")]
    WideArmsThenCrouchAndEmitUpwards = 91,
    [Description("Swing Back, then Hand Upwards")]
    SwingBackThenHandUpwards = 92,
    [Description("Stagger Hand Up, then Front Sweep")]
    StaggerHandUpThenFrontSweep = 93,
    [Description("Claw Motions")]
    ClawMotions = 94,
    [Description("Raise Hand, then Swish")]
    RaiseHandThenSwish95 = 95,
    [Description("Crouch, then Leap and Throw Dual Spears (Lightning)")]
    CrouchThenLeapAndThrowDualSpearsLightning = 98,
    [Description("Hand to Ground, then Swipe Up")]
    HandToGroundThenSwipeUp99 = 99,
    [Description("Erupt from Chest, then Stride Forward")]
    EruptFromChestThenStrideForward = 100,
    [Description("Hold Above, then Slam Down")]
    HoldAboveThenSlamDown = 102,
    [Description("Raise Hand, then Swish")]
    RaiseHandThenSwish103 = 103,
    [Description("Raise Above, then Lower and Burst")]
    RaiseAboveThenLowerAndBurst = 104,
    [Description("Hands Forward, then Explode")]
    HandsForwardThenExplode = 105,
    [Description("Hand to Head, then Flick Wrist")]
    HandToHeadThenFlickWrist = 106,
    [Description("Raise Hand, then Wide Swish")]
    RaiseHandThenWideSwish = 107,
    [Description("Side Hold, Sweep Front")]
    SideHoldSweepFront108 = 108,
    [Description("Side Hold, Sweep Above")]
    SideHoldSweepAbove = 109,
    [Description("Raise Hand, then Swish")]
    RaiseHandThenSwish110 = 110,
    [Description("Hand to Head, then Thrust Up")]
    HandToHeadThenThrustUp = 111,
    [Description("Enchant - Weapon")]
    EnchantWeapon112 = 112,
    [Description("Side Hold, Sweep Front")]
    SideHoldSweepFront113 = 113,
    [Description("Hand to Head, then Hunch")]
    HandToHeadThenHunch = 114,
    [Description("Hold Above, then Slam")]
    HoldAboveThenSlam115 = 115,
    [Description("Sweep Back, then Spinning Slash")]
    SweepBackThenSpinningSlash = 116,
    [Description("Side Hold, Cast Forward")]
    SideHoldCastForward = 117,
    [Description("Raise Hand, then Quick Swish")]
    RaiseHandThenQuickSwish118 = 118,
    [Description("Swing Back, then Jump and Slam")]
    SwingBackThenJumpAndSlam = 119,
    [Description("Hold Above, then Slam")]
    HoldAboveThenSlam120 = 120,
    [Description("Hold Above, then Slam")]
    HoldAboveThenSlam121 = 121,
    [Description("Hunch, then Floating Leap and Slam")]
    HunchThenFloatingLeapAndSlam = 122,
    [Description("Magma Breath")]
    MagmaBreath = 123,
    [Description("Raise Hand, then Quick Swish")]
    RaiseHandThenQuickSwish124 = 124
}