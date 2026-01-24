using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BuddyParam;

public enum BuddyPcFollowType : byte
{
    [Description("Follow Warp To Player (Needs SpEffect 297000)")]
    FollowWarpToPlayerNeedsSpEffect297000 = 0,
    Wander = 1,
    StayStill = 2
}