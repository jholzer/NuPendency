using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NuPendency.Interfaces.Model;

namespace NuPendency.Gui.Converters
{
    public class PackageTypeToBorderThicknessConverter : IValueConverter
    {
        public int BorderThicknessRootValue { get; set; } = 2;
        public int BorderThicknessNormalValue { get; set; } = 1;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RootNuGetPackage)
                return BorderThicknessRootValue;
            return BorderThicknessNormalValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}