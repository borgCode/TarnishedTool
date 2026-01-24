using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.BulletParam;

public enum BulletBallisticCalcType : byte
{
    [Description("Emits Forward")]
    EmitsForward = 0,
    [Description("Emits Backward")]
    EmitsBackward = 1
}