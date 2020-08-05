using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ChatDesktop.Converters
{
    public class BooleanToVisibilityInverter : IValueConverter
    {
        private object GetVisibility(object value)
        {
            if (!(value is bool))
                return Visibility.Collapsed;
            bool objValue = !(bool)value;
            if (objValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
