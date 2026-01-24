// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.SpEffect;

public enum SpEffectDmgTypeNew : byte
{
    Default = 0,
    [Description("No Stagger [0: NONE]")]
    NoStagger = 1,
    [Description("Minimum Stagger [8: MINIMUM]")]
    MinimumStagger = 2,
    [Description("Short Stagger [1: SMALL]")]
    ShortStagger = 3,
    [Description("Medium Stagger [2: MEDIUM]")]
    MediumStagger = 4,
    [Description("Long Stagger [3: LARGE]")]
    LongStagger = 5,
    [Description("Launch Back [7: SMALL_BLOW]")]
    LaunchBack = 6,
    [Description("Big Launch Back [4: EXLARGE]")]
    BigLaunchBack = 7,
    [Description("Pancake [6: FLING]")]
    Pancake = 8,
    [Description("Air Juggle [9: UPPER]")]
    AirJuggle = 9,
    [Description("Push [5: PUSH]")]
    Push = 10,
    [Description("Huge Launch Back [10: EX_BLAST]")]
    HugeLaunchBack = 11,
    [Description("Flailing Arms [11: BREATH]")]
    FlailingArms = 12
}