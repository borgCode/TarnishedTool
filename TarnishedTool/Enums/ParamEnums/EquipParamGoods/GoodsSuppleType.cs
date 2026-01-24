// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamGoods;

public enum GoodsSuppleType : byte
{
    [Description("Type 0")]
    Type0 = 0,
    [Description("Type 1")]
    Type1 = 1,
    [Description("Type 2 - Crimson Flask")]
    Type2CrimsonFlask = 2,
    [Description("Type 3 - Cerulean Flask")]
    Type3CeruleanFlask = 3
}