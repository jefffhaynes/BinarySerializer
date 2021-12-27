namespace BinarySerialization.Test.Issues.Issue82;

public class SerializeWhenClass
{
    [FieldOrder(0)]
    public int Version { get; set; }

    [FieldOrder(1)]
    [SerializeWhen("Version", true, ConverterType = typeof(VersionBoolConverter), ConverterParameter = 30)]
    public bool Value { get; set; }
}
