using BinarySerialization;

namespace BinarySerializer.Performance
{
    public class TwiceConverter : IValueConverter
    {
        public object Convert(object value, object converterParameter, BinarySerializationContext ctx)
        {
            var a = System.Convert.ToInt32(value);
            return a*2;
        }

        public object ConvertBack(object value, object converterParameter, BinarySerializationContext ctx)
        {
            var a = System.Convert.ToInt32(value);
            return a/2;
        }
    }
}