namespace BinarySerialization.Test.Issues.Issue30;

public class CommMessage<T> : IMessage<T> where T : class, IPayload
{
    public CommMessage()
    {
    }

    public CommMessage(Header header, T payload)
    {
        Header = header;
        Payload = payload;
    }

    [FieldOrder(0)]
    public Header Header { get; set; }

    [FieldOrder(1)]
    public T Payload { get; set; }

    public void ComplementHeader()
    {
        Header.PayloadType = Payload.GetPayloadType();
        Header.PayloadSize = Payload.GetPayloadLength();
    }
}
