using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletSfxCreatePostureType : sbyte
{
    Parent = 0,
    [Description("Global Y Axis")]
    GlobalYAxis = 1,
    Impact = 2
}