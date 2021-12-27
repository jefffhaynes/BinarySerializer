namespace BinarySerialization.Test.Issues.Issue151;

public class Packet
{
    [FieldOrder(0)]
    public byte ProtocolVersion { get; set; }

    [FieldOrder(1)]
    public byte InvProtocolVersion { get; set; }

    [FieldOrder(2)]
    public dPayloadType PayloadType { get; set; }

    [FieldOrder(3)]
    public UInt32 PayloadLength { get; set; }

    [FieldOrder(4)]
    [FieldLength(nameof(PayloadLength))]
    [Subtype(nameof(PayloadType), dPayloadType.GenericNACK, typeof(GenericNACKPayload))]
    [Subtype(nameof(PayloadType), dPayloadType.Request, typeof(Request))]
    [Subtype(nameof(PayloadType), dPayloadType.UserData, typeof(UserData))]
    public dPayload Payload { get; set; }
}
