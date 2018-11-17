using System;

namespace BinarySerialization.Test.Endianness
{
    public class InvalidEndiannessConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            return 0;
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
