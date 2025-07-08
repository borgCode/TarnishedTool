using System;
using System.Globalization;
using System.Windows.Data;

namespace SilkyRing.Converters
{
    public class ImmuneFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isImmune = (bool)value;
            string normalColor = parameter as string ?? "#CE93D8";
        
            return isImmune ? "#888888" : normalColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}