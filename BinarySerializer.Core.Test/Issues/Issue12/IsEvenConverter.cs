using System;

namespace BinarySerialization.Test.Issues.Issue12
{
    public class IsEvenConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            var intValue = System.Convert.ToInt32(value);
            return intValue%2 == 0;
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
