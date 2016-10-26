using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NuPendency.Gui.Converters
{
    public class PackageTypeToVisibilityConverter : IValueConverter
    {
        public Type MatchingType { get; set; }
        public Visibility MatchVisibility { get; set; }
        public Visibility NoMatchVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == MatchingType)
                return MatchVisibility;
            return NoMatchVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}