namespace BinarySerialization.Test;

public class DoubleOutlierConverter : IValueConverter
{
    public object Convert(object value, object converterParameter, BinarySerializationContext ctx)
    {
        var a = System.Convert.ToDouble(value);
        return a * 2;
    }

    public object ConvertBack(object value, object converterParameter, BinarySerializationContext ctx)
    {
        var a = System.Convert.ToDouble(value);
        return a / 2;
    }
}
