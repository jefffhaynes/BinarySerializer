using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerialization.Test.Issues.Issue61
{
    public class Converter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
