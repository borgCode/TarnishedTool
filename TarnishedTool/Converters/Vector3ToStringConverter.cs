// 

using System;
using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace TarnishedTool.Converters;


public class Vector3ToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Vector3 v)
            return $"{v.X:F2}  {v.Y:F2}  {v.Z:F2}";
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}