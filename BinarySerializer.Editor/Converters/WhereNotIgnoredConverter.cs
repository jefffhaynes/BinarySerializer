using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.Converters
{
    public class WhereNotIgnoredConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var typeNodes = (IEnumerable<TypeNode>)value;
            return typeNodes.Where(node => node.IgnoreAttribute == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
