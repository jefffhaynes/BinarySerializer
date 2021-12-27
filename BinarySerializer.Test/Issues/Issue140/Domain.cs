namespace BinarySerialization.Test.Issues.Issue140;

public class Domain : IBinarySerializable, IDomain
{
    [Ignore]
    public List<ILabel> Labels { get; }

    [Ignore]
    public string Name => ToString();

    public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext context)
    {
        //Code
    }

    public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext context)
    {
        //Code
    }

}
