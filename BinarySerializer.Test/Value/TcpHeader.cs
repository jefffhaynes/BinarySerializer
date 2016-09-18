using System.Collections.Generic;

namespace BinarySerialization.Test.Value
{
    // TODO not tested
    public class TcpHeader
    {
        [FieldOrder(0)]
        public ushort SourcePort { get; set; }

        [FieldOrder(1)]
        public ushort DestinationPort { get; set; }

        [FieldOrder(2)]
        public uint SequenceNumber { get; set; }

        [FieldOrder(3)]
        public uint AckNumber { get; set; }

        [FieldOrder(4)]
        public byte Offset { get; set; }

        [FieldOrder(5)]
        public TcpHeaderFlags Flags { get; set; }

        [FieldOrder(6)]
        public ushort WindowSize { get; set; }

        [FieldOrder(7)]
        public ushort Checksum { get; set; }

        [FieldOrder(8)]
        public ushort UrgentPointer { get; set; }

        [FieldOrder(9)]
        [FieldAlignment(4)]
        [FieldLength("Offset", ConverterType = typeof(OffsetLengthConverter))]
        [ItemSerializeUntil("Kind", TcpOptionKind.End)]
        public List<TcpOption> Options { get; set; }
    }
}