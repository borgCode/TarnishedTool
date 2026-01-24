// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.ItemLotParam;

public enum ItemLotItemCategory : uint
{
    None = 0,
    Good = 1,
    Weapon = 2,
    Armor = 3,
    Accessory = 4,
    [Description("Ash of War")]
    AshOfWar = 5,
    [Description("Custom Weapon")]
    CustomWeapon = 6
}