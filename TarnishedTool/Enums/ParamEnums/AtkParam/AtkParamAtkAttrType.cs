// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamAtkAttrType : byte
{
    Slash = 0,
    Strike = 1,
    Pierce = 2,
    Standard = 3,
    [Description("EquipParamWeapon atkAttribute2 Reference")]
    EquipParamWeaponAtkAttribute2Reference = 252,
    [Description("EquipParamWeapon atkAttribute Reference")]
    EquipParamWeaponAtkAttributeReference = 253,
    None = 254
}