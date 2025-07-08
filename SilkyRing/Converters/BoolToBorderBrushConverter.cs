using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SilkyRing.Converters
{
    public class BoolToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Red : Brushes.DarkGray;
        }
   
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}