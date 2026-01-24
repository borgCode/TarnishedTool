using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BehaviorParam;

public enum BehaviorCategory : byte
{
    [Description("No Category")]
    NoCategory = 0,
    [Description("PC Right Hand")]
    PcRightHand = 1,
    [Description("PC Left Hand")]
    PcLeftHand = 2,
    [Description("Magic 1")]
    Magic1 = 3,
    [Description("Magic 2")]
    Magic2 = 4,
    Basic = 5,
    [Description("NPC Right Hand")]
    NpcRightHand = 6,
    [Description("NPC Left Hand")]
    NpcLeftHand = 7,
    Kick = 9,
    [Description("PC Both Hand 1")]
    PcBothHand1 = 10,
    [Description("PC Both Hand 2")]
    PcBothHand2 = 11,
    [Description("PC Right Hand 2")]
    PcRightHand2 = 12
}