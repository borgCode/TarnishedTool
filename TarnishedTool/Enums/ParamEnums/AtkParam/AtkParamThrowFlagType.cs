using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamThrowFlagType : byte
{
    None = 0,
    [Description("Throw Transition")] 
    ThrowTransition = 1,
    Throw = 2,
    [Description("Throw Transition With Damage Level")]
    ThrowTransitionWithDmgLevel = 3,

}