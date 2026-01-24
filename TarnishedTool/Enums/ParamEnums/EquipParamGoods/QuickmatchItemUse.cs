// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamGoods;

public enum QuickmatchItemUse : byte
{
    [Description("Always Allowed")]
    AlwaysAllowed = 0,
    [Description("Prohibited In Duel")]
    ProhibitedInDuel = 1,
    [Description("Prohibited In Team Match")]
    ProhibitedInTeamMatch = 2,
    [Description("Prohibited In Brawl and Team Match")]
    ProhibitedInBrawlAndTeamMatch = 3,
    [Description("Prohibited In Duel, Brawl, and Team Match")]
    ProhibitedInDuelBrawlAndTeamMatch = 4,
    [Description("Prohibited In Duel and Team Match")]
    ProhibitedInDuelAndTeamMatch = 5,
    [Description("Prohibited In Brawl and Team Match")]
    ProhibitedInBrawlAndTeamMatch2 = 6,
    [Description("Always Prohibited")]
    AlwaysProhibited = 7
}