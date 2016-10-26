using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace NuPendency.Gui.Converters
{
    public class PackageDepthToOpacityConverter : IValueConverter
    {
        public double Step { get; set; } = 0.05;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return 1;
            double depth = System.Convert.ToDouble(value);
            return 1 - Step * depth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}