using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamBehaviorId
{
    None = 0,
    [Description("Continuous Attack")]
    ContinuousAttack = 1,
    [Description("Super Huge Blowout")]
    SuperHugeBlowout = 2,
    [Description("Delayed Damage Reaction")]
    DelayedDamageReaction = 3,
    [Description("Damage Only FE")]
    DamageOnlyFE = 4,
    [Description("Small Stagger Guarantee")]
    SmallStaggerGuarantee = 5,
    Unknown6 = 6,
    Unknown7 = 7,
    Unknown8 = 8,
    Unknown9 = 9,
    [Description("Start Grab")]
    StartGrab = 10,
    [Description("Finish Grab")]
    FinishGrab = 11
}