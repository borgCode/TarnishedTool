// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum GemMountType : byte
{
    [Description("Prevent Change")]
    PreventChange = 0,
    [Description("Allow Change")]
    AllowChange = 2
}