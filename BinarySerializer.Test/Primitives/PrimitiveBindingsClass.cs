namespace BinarySerialization.Test.Primitives
{
    public class PrimitiveBindingsClass
    {
        [FieldOrder(0)]
        public byte ByteLength { get; set; }

        [FieldOrder(1)]
        public sbyte SByteLength { get; set; }

        [FieldOrder(2)]
        public short ShortLength { get; set; }

        [FieldOrder(3)]
        public ushort UShortLength { get; set; }

        [FieldOrder(4)]
        public int IntLength { get; set; }

        [FieldOrder(5)]
        public uint UIntLength { get; set; }

        [FieldOrder(6)]
        public long LongLength { get; set; }

        [FieldOrder(7)]
        public ulong ULongLength { get; set; }

        [FieldOrder(8)]
        public float FloatLength { get; set; }

        [FieldOrder(9)]
        public double DoubleLength { get; set; }

        [FieldOrder(10)]
        public char CharLength { get; set; }

        [FieldOrder(11)]
        [FieldLength(nameof(ByteLength))]
        [FieldLength(nameof(SByteLength))]
        [FieldLength(nameof(ShortLength))]
        [FieldLength(nameof(UShortLength))]
        [FieldLength(nameof(IntLength))]
        [FieldLength(nameof(UIntLength))]
        [FieldLength(nameof(LongLength))]
        [FieldLength(nameof(ULongLength))]
        [FieldLength(nameof(FloatLength))]
        [FieldLength(nameof(DoubleLength))]
        [FieldLength(nameof(CharLength))]
        public string Value { get; set; }
    }
}
