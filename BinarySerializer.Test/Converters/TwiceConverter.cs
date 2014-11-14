using BinarySerialization;

namespace BinarySerializer.Test.Converters
{
    public class TwiceConverter : IValueConverter
    {
        public object Convert(object value, BinarySerializationContext ctx)
        {
            var a = System.Convert.ToDouble(value);
            return a * 2;
        }

        public object ConvertBack(object value, BinarySerializationContext ctx)
        {
            var a = System.Convert.ToDouble(value);
            return a / 2;
        }
    }
}