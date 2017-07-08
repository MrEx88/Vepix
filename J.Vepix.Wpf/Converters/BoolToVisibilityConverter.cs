using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace J.Vepix.Wpf.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
         => (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
         => value is Visibility && (Visibility)value == Visibility.Visible;
    }
}
