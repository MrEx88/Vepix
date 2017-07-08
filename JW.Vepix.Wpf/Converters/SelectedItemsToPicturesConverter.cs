using JW.Vepix.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace JW.Vepix.Wpf.Converters
{
    public class SelectedItemsToPicturesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pictures = new List<Picture>();
            var list = value as IList;
            foreach (var item in list)
            {
                pictures.Add(item as Picture);
            }
            return pictures;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
