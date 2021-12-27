namespace BinarySerialization.Test.Issues.Issue50;

public class MsgHeader
{
    [FieldOrder(0)]
    [SerializeAs(SerializedType = SerializedType.UInt2)]
    public PlcMessageType MsgType { get; set; }

    [FieldOrder(1)]
    [SerializeAs(SerializedType = SerializedType.UInt4)]
    public PlcPayloadType PayloadType { get; set; }
}
