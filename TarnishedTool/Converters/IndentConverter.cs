// 

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TarnishedTool.Converters;

public class IndentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int indent = (int)value;
        return new Thickness(indent * 20, 0, 0, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}