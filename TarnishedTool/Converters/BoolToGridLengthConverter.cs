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
            if (value is bool isVisible)
            {
                return isVisible ? new GridLength(150) : new GridLength(0);
            }
            return new GridLength(150);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}