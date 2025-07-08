using System;
using System.Windows;
using System.Windows.Data;

namespace SilkyRing.Converters
{
    public class GreaterThanZeroVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int intValue && intValue > 0)
                return Visibility.Visible;
        
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}