// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum ReinforceShopCategory : byte
{
    [Description("No Reinforcement")]
    NoReinforcement = 0,
    Weapon = 1,
    [Description("Sorcery Catalyst")]
    SorceryCatalyst = 2,
    [Description("Incantation Catalyst")]
    IncantationCatalyst = 3
}