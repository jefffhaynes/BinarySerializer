using System;

namespace BinarySerialization.Test.Encoding
{
    public class EncodingConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            var encodingName = (string) value;
            return EncodingHelper.GetEncoding(encodingName);
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotSupportedException();
        }
    }
}
