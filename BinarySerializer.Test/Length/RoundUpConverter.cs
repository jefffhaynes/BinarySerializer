namespace BinarySerialization.Test.Length;

public class RoundUpConverter : IValueConverter
{
    public object Convert(object value, object parameter, BinarySerializationContext context)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));

        var v = System.Convert.ToUInt64(value);
        var m = System.Convert.ToUInt64(parameter);
        return v + (m - v % m) % m;
    }

    public object ConvertBack(object value, object parameter, BinarySerializationContext context)
    {
        throw new NotSupportedException();
    }
}
