using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace NuPendency.Gui.Converters
{
    public class NegativeIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int iVal = System.Convert.ToInt32(value);
                return -iVal;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}