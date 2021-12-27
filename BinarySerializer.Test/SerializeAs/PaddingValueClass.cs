namespace BinarySerialization.Test.SerializeAs;

class PaddingValueClass
{
    [FieldLength(5)]
    [SerializeAs(PaddingValue = 0x33)]
    public string Value { get; set; }
}
