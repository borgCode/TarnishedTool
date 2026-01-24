// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamAccessory;


public enum DisplayCategoryIdAccessory : byte
{
    [Description("Group 10 - Core Stats")]
    Group10CoreStats = 10,
    [Description("Group 20 - Resistances")]
    Group20Resistances = 20,
    [Description("Group 30 - Weapon")]
    Group30Weapon = 30,
    [Description("Group 40 - Spell")]
    Group40Spell = 40,
    [Description("Group 50 - Circumstantial")]
    Group50Circumstantial = 50,
    [Description("Group 60 - Armor")]
    Group60Armor = 60,
    [Description("Group 70 - Restore/Gain")]
    Group70RestoreGain = 70,
    [Description("Group 80 - Unique")]
    Group80Unique = 80,
    None = 255
}