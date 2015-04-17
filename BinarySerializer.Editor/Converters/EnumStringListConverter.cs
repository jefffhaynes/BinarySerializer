using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace BinarySerializer.Editor.Converters
{
    public class EnumStringListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enums = (IEnumerable<Enum>) value;
            var strings = enums.Select(e => string.Format("0x{0} -> {1}", System.Convert.ToInt64(e).ToString("X2"), e.ToString()));
            return string.Join(", ", strings);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
