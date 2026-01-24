// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamGoods;

public enum GoodsType : byte
{
    [Description("Normal Item")]
    NormalItem = 0,
    [Description("Key Item")]
    KeyItem = 1,
    [Description("Crafting Material")]
    CraftingMaterial = 2,
    Remembrance = 3,
    None4 = 4,
    Sorcery = 5,
    None6 = 6,
    [Description("Spirit Summon - Lesser")]
    SpiritSummonLesser = 7,
    [Description("Spirit Summon - Greater")]
    SpiritSummonGreater = 8,
    [Description("Wondrous Physick")]
    WondrousPhysick = 9,
    [Description("Wondrous Physick Tear")]
    WondrousPhysickTear = 10,
    [Description("Regenerative Material")]
    RegenerativeMaterial = 11,
    [Description("Info Item")]
    InfoItem = 12,
    None13 = 13,
    [Description("Reinforcement Material")]
    ReinforcementMaterial = 14,
    [Description("Great Rune")]
    GreatRune = 15,
    Incantation = 16,
    [Description("Self Buff - Sorcery")]
    SelfBuffSorcery = 17,
    [Description("Self Buff - Incantation")]
    SelfBuffIncantation = 18
}