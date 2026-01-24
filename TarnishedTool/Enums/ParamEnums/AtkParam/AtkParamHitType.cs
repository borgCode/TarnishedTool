using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamHitType : byte
{
    [Description("Normal Tip")]
    NormalTip = 0,
    Middle = 1,
    Root = 2,
    [Description("Map Collision Detection")]
    MapCollisionDetection = 3
}