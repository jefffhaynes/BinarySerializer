namespace BinarySerialization.Test.Issues.Issue76
{
    public class PacketContent
    {
        [FieldOrder(0)]
        [SerializeAs(SerializedType.TerminatedString, StringTerminator = 0x20)]
        public CommandClass CommandClass { get; set; }

        [FieldOrder(1)]
        [Subtype("CommandClass", CommandClass.Smn, typeof(SmnCommandContainer))]
        public CommandContainer Payload { get; set; }
    }
}