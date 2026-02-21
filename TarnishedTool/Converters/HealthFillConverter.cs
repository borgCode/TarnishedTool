using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TarnishedTool.Converters;

public class HealthFillConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is double current && values[1] is double max && max > 0)
        {
            double ratio = Math.Max(0, Math.Min(1, current / max));


            double hue = Math.Pow(ratio, 1.35) * 120.0;
            double saturation = 0.65 + 0.35 * Math.Pow(1 - ratio, 1);
            double brightness = 0.65 + 0.25 * Math.Pow(1 - ratio, 1);
            var color = HsvToRgb(hue, saturation, brightness);
            return new SolidColorBrush(color);
        }

        return new SolidColorBrush(Color.FromRgb(76, 170, 80));
    }
    
    private Color HsvToRgb(double h, double s, double v)
    {
        int hi = (int)(h / 60) % 6;
        double f = h / 60 - Math.Floor(h / 60); 
        double p = v * (1 - s); 
        double q = v * (1 - f * s);
        double t = v * (1 - (1 - f) * s);

        double r, g, b;
        switch (hi)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            default:
                r = v;
                g = p;
                b = q;
                break;
        }

        return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}