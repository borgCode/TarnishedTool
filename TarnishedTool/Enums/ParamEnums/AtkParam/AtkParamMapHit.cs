using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.AtkParam;

public enum AtkParamMapHit : byte
{
    Normal = 0,
    Precision = 1,
    [Description("Not On The Map")]
    NotOnTheMap = 2,
    [Description("Map Not An Asset")]
    MapNotAnAsset = 3
}