using NuPendency.Gui.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NuPendency.Gui.Converters
{
    public class SelectionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SelectionMode selectionMode = value as SelectionMode? ?? SelectionMode.NotSelected;
            if (selectionMode == SelectionMode.Selected)
            {
                return Brushes.GreenYellow;
            }

            if (selectionMode == SelectionMode.Highlighted)
            {
                return Brushes.Orange;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}