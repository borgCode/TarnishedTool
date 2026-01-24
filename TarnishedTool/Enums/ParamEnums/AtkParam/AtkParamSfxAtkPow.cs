using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamSfxAtkPow : sbyte
{
    Small = 0,
    Medium = 1,
    Large = 2,
    [Description("Extra Large")] 
    ExtraLarge = 3,
    [Description("Very Large")] 
    VeryLarge = 4
}