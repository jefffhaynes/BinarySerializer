namespace BinarySerialization.Test.Value
{
    public class BitMaskConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            var intValue = System.Convert.ToUInt32(value);
            var mask = System.Convert.ToUInt32(parameter);
            return intValue & mask;
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            return value;
        }
    }
}
