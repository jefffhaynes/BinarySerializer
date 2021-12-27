namespace BinarySerialization.Test.Issues.Issue82;

class VersionBoolConverter : IValueConverter
{
    public object Convert(object value, object parameter, BinarySerializationContext context)
    {
        var version = (int)value;
        var minVersion = (int)parameter;

        return version > minVersion;
    }

    public object ConvertBack(object value, object parameter, BinarySerializationContext context)
    {
        throw new NotSupportedException();
    }
}
