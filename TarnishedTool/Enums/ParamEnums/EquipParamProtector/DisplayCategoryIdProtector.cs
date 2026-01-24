// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamProtector;

public enum DisplayCategoryIdProtector : byte
{
    [Description("Lightweight #1")]
    Lightweight1 = 10,
    [Description("Lightweight #2")]
    Lightweight2 = 20,
    [Description("Lightweight #3")]
    Lightweight3 = 30,
    [Description("Lightweight #4")]
    Lightweight4 = 40,
    [Description("Mediumweight #1")]
    Mediumweight1 = 50,
    [Description("Mediumweight #2")]
    Mediumweight2 = 60,
    [Description("Mediumweight #3")]
    Mediumweight3 = 70,
    [Description("Heavyweight #1")]
    Heavyweight1 = 80,
    [Description("Heavyweight #2")]
    Heavyweight2 = 90,
    [Description("Heavyweight #3")]
    Heavyweight3 = 100,
    [Description("Heavyweight #4")]
    Heavyweight4 = 110,
    None = 255
}