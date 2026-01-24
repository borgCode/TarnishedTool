// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace TarnishedTool.Utilities;

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return (T[])Enum.GetValues(typeof(T));
    }
    
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? value.ToString();
    }
}