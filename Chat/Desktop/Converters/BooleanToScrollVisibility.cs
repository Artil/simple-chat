using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChatDesktop.Converters
{
    public class BooleanToScrollVisibility : IValueConverter
    {
        private object GetVisibility(object value)
        {
            if (!(value is bool))
                return ScrollBarVisibility.Hidden;
            var objValue = (bool)value;
            if (objValue)
            {
                return ScrollBarVisibility.Hidden;
            }
            return ScrollBarVisibility.Auto;
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
