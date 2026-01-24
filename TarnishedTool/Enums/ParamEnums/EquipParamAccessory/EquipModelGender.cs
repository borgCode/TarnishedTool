// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamAccessory;

public enum EquipModelGender : byte
{
    None = 0,
    Male = 1,
    Female = 2,
    Unknown = 3,
    [Description("No Gender")]
    NoGender = 4
}