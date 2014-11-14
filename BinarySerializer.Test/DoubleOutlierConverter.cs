using BinarySerialization;

namespace BinarySerializer.Test
{
    public class DoubleOutlierConverter : IValueConverter
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
