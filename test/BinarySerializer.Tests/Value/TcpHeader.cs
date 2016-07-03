namespace BinarySerialization.Test.Value
{
    // TODO not finished
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
        public ushort DataOffsetAndFlags { get; set; }

        [FieldOrder(5)]
        public TcpHeaderFlags Flags { get; set; }

        [FieldOrder(6)]
        public ushort WindowSize { get; set; }

        [FieldOrder(7)]
        public ushort Checksum { get; set; }

        [FieldOrder(8)]
        public ushort UrgentPointer { get; set; }

        [FieldOrder(9)]
        [FieldLength("DataOffsetAndFlags", ConverterType = typeof(BitMaskConverter), ConverterParameter = 0xf0)]
        public uint[] Options { get; set; }
    }
}
