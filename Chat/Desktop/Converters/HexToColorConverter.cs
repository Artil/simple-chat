using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatDesktop.Converters
{
    class HexToColorConverter : IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var col = ColorTranslator.FromHtml(value?.ToString() ?? "#FF1F2020");
                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(col.A, col.R, col.G, col.B));
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                SolidColorBrush c = (SolidColorBrush)value;
                return ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c.Color.A, c.Color.R, c.Color.G, c.Color.B));
            }
    }
}
