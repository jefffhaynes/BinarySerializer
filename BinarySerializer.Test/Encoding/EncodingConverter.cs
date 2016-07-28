using System;

namespace BinarySerialization.Test.Encoding
{
    public class EncodingConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            return System.Text.Encoding.GetEncoding((string) value);
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotSupportedException();
        }
    }
}
