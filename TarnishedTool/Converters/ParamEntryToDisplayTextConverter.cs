// 

using System;
using System.Globalization;
using System.Windows.Data;
using TarnishedTool.Models;

namespace TarnishedTool.Converters;

public class ParamEntryToDisplayTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ParamEntry entry)
            return string.Empty;
            
        return entry.HasName 
            ? $"{entry.Parent}: {entry.Id} - {entry.DisplayName}" 
            : $"{entry.Parent}: {entry.Id}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}