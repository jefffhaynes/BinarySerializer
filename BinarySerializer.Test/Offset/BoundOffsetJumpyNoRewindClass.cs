using System.IO;

namespace BinarySerialization.Test.Offset
{
    public class BoundOffsetJumpyNoRewindClass
    {
        [FieldOrder(0)]
        public uint FieldOffsetField1 { get; set; }

        [FieldOrder(1)]
        [FieldOffset(nameof(FieldOffsetField1), false)]
        public uint Field1 { get; set; }

        [FieldOrder(2)]
        public uint FieldOffsetField2 { get; set; }

        [FieldOrder(3)]
        [FieldOffset(nameof(FieldOffsetField2), false)]
        public uint Field2 { get; set; }

        [FieldOrder(4)]
        public uint FieldOffsetField3 { get; set; }

        [FieldOrder(5)]
        [FieldOffset(nameof(FieldOffsetField3), SeekOrigin.Current, false)]
        public uint Field3 { get; set; }
    }
}