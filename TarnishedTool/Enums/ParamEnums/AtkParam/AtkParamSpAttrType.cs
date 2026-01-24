// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamSpAttrType : byte
{
    None = 0,
    Magic = 10,
    Fire = 11,
    Lightning = 12,
    Holy = 13,
    Poison = 20,
    [Description("Scarlet Rot")]
    ScarletRot = 21,
    Bloodloss = 22,
    Frostbite = 23,
    Sleep = 24,
    Madness = 25,
    [Description("Death Blight")]
    DeathBlight = 26,
    None254 = 254
}