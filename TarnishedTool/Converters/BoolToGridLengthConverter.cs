using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TarnishedTool.Converters
{
    public class BoolToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool show && show)
            {
                return new GridLength(130, GridUnitType.Pixel); // Fixed when visible
            }

            return new GridLength(0, GridUnitType.Pixel); // Hidden
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}