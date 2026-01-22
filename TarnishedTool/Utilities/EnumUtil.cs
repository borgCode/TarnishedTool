// 

using System;
using System.Collections.Generic;

namespace TarnishedTool.Utilities;

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return (T[])Enum.GetValues(typeof(T));
    }
}