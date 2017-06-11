using Jw.Vepix.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Jw.Vepix.Wpf.Converters
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
