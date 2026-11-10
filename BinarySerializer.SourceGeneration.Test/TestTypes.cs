using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerialization.SourceGeneration.Test
{
    /* Seeded automatically: carries BinarySerializer attributes. */

    public class SimplePacket
    {
        [FieldOrder(0)]
        public ushort A { get; set; }

        [FieldOrder(1)]
        public byte B { get; set; }
    }

    public class FieldPacket
    {
        [FieldOrder(0)]
        public ushort Id;

        [FieldOrder(1)]
        public byte Flags;
    }

    public class ImmutablePoint
    {
        public ImmutablePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        [FieldOrder(0)]
        public int X { get; }

        [FieldOrder(1)]
        public int Y { get; }
    }

    public enum ItemKind : byte
    {
        None = 0,
        Alpha = 1,
        Beta = 2
    }

    public class EnumItem
    {
        [FieldOrder(0)]
        public ItemKind Kind { get; set; }

        [FieldOrder(1)]
        public ushort Value { get; set; }
    }

    public class EnumListPacket
    {
        [FieldOrder(0)]
        public byte Count { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<EnumItem> Items { get; set; }
    }

    public abstract class Frame
    {
    }

    public class FrameA : Frame
    {
        [FieldOrder(0)]
        public ushort A { get; set; }
    }

    public class FrameB : Frame
    {
        [FieldOrder(0)]
        public int B { get; set; }
    }

    public class SubtypePacket
    {
        [FieldOrder(0)]
        public byte Type { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(Type), 1, typeof(FrameA))]
        [Subtype(nameof(Type), 2, typeof(FrameB))]
        public Frame Payload { get; set; }
    }

    public class InitPacket
    {
        [FieldOrder(0)]
        public int A { get; init; }

        [FieldOrder(1)]
        public int B { get; init; }
    }

    public class BasePacket
    {
        [FieldOrder(0)]
        public byte BaseField { get; set; }
    }

    public class DerivedPacket : BasePacket
    {
        [FieldOrder(1)]
        public byte DerivedField { get; set; }
    }

    public class StringPacket
    {
        [FieldOrder(0)]
        public ushort Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length))]
        public string Text { get; set; }
    }

    public class PrimitiveListPacket
    {
        [FieldOrder(0)]
        public byte Count { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<byte> Data { get; set; }
    }

    public class NestedOuter
    {
        public class NestedDto
        {
            [FieldOrder(0)]
            public byte Value { get; set; }
        }

        [FieldOrder(0)]
        public NestedDto Inner { get; set; }
    }

    /// <summary>
    ///     Serialize-only probe for accessor registration (contains a get-only property that is not
    ///     a constructor parameter, which the framework intentionally rejects on deserialization).
    /// </summary>
    public class AccessorProbe
    {
        public AccessorProbe()
        {
            ReadOnlyValue = 9;
        }

        [FieldOrder(0)]
        public int Value { get; set; }

        [FieldOrder(1)]
        public int Number;

        [FieldOrder(2)]
        public int ReadOnlyValue { get; }
    }

    /* Seeded explicitly: no BinarySerializer attributes at all. */

    [BinarySerializerRoot]
    public class RootOnlyPacket
    {
        public int Value { get; set; }
    }

    [BinarySerializerRoot]
    public class FieldNullProbe
    {
        public string Label;
    }
}
