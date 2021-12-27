namespace BinarySerializer.Performance;

[Serializable]
public class BasicClass
{
    [FieldOrder(0)]
    public byte ByteValue { get; set; }

    [FieldOrder(1)]
    public short ShortValue { get; set; }

    [FieldOrder(2)]
    public int IntValue { get; set; }

    [FieldOrder(3)]
    public string StringValue { get; set; }
}
