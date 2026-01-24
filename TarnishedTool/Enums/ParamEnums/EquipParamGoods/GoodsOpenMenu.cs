// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamGoods;

public enum GoodsOpenMenu : byte
{
    None = 0,
    [Description("Common Interaction")]
    CommonInteraction = 1,
    [Description("Leave World")]
    LeaveWorld = 4,
    [Description("Write Message")]
    WriteMessage = 5,
    [Description("Invade World (Bloody Finger)")]
    InvadeWorldBloodyFinger = 10,
    [Description("Beckon Spirit Summon")]
    BeckonSpiritSummon = 13,
    [Description("Reveal Summon Signs")]
    RevealSummonSigns = 14,
    [Description("Request Aid")]
    RequestAid = 15,
    [Description("Answer Aid Request")]
    AnswerAidRequest = 16,
    [Description("Encourage Invasion")]
    EncourageInvasion = 17,
    [Description("Add Summon Sign to Pool")]
    AddSummonSignToPool = 18,
    [Description("Add Duel Sign to Pool")]
    AddDuelSignToPool = 19,
    [Description("Place Summon Sign")]
    PlaceSummonSign = 20,
    [Description("Invade World (Recusant Finger)")]
    InvadeWorldRecusantFinger = 21,
    [Description("Invade World (Festering Bloody Finger)")]
    InvadeWorldFesteringBloodyFinger = 22
}