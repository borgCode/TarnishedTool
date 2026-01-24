// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamGoods;

public enum AutoReplenishType : byte
{
    Type0 = 0,
    [Description("Type 1 - Rune/Material")]
    Type1RuneMaterial = 1,
    [Description("Type 2 - Consumable")]
    Type2Consumable = 2
}