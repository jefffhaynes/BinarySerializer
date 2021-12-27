namespace BinarySerialization.Test.Issues.Issue30;

public class MessageSerializer
{
    private readonly BinarySerializer _binSerializer;

    public MessageSerializer()
    {
        _binSerializer = new BinarySerializer { Endianness = BinarySerialization.Endianness.Big };
    }

    public byte[] BinarySerializeMessage<T>(IMessage<T> message) where T : class, IPayload
    {
        message.ComplementHeader();

        using (var memoryStream = new MemoryStream())
        {
            _binSerializer.Serialize(memoryStream, message);
            return memoryStream.ToArray();
        }
    }

    public object BinaryDeserializeMessage(byte[] binBytes, Type type)
    {
        return _binSerializer.Deserialize(binBytes, type);
    }
}
