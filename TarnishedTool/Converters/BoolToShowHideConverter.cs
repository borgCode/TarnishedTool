using System;
using System.Globalization;
using System.Windows.Data;

namespace TarnishedTool.Converters
{
    public class BoolToShowHideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isShowing)
            {
                return isShowing ? "Hide Vanilla Values" : "Show Vanilla Values";
            }
            return "Show Vanilla Values";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}