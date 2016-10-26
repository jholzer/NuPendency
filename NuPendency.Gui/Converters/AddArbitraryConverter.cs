using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using NuPendency.Gui.ViewModels;

namespace NuPendency.Gui.Converters
{
    public class AddArbitraryConverter : IValueConverter
    {
        public int Arbitrary { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue;
            try
            {
                doubleValue = System.Convert.ToDouble(value);
            }
            catch (Exception)
            {
                return value;
            }
            
            return doubleValue + Arbitrary/2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
