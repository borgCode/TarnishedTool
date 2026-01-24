// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamAccessory;

public enum BehaviorRefType : byte
{
    [Description("Behavior / None")]
    BehaviorNone = 0,
    Bullet = 1,
    SpEffect = 2
}