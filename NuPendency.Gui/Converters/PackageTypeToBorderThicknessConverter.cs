using NuPendency.Interfaces.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NuPendency.Gui.Converters
{
    public class PackageTypeToBorderThicknessConverter : IValueConverter
    {
        public int BorderThicknessNormalValue { get; set; } = 1;
        public int BorderThicknessRootValue { get; set; } = 2;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pack = value as PackageBase;
            if ((pack != null) && (pack.Depth == 0))
                return BorderThicknessRootValue;
            return BorderThicknessNormalValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}