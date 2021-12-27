namespace BinarySerialization.Test.UntilItem;

public class UntilItemSimpleClass
{
    [FieldOrder(0)]
    public UntilItemEnum Type { get; set; }

    [FieldOrder(1)]
    public byte Value { get; set; }
}
