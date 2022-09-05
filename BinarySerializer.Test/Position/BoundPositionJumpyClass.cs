using System.IO;

namespace BinarySerialization.Test.Position
{
    public class BoundPositionJumpyClass
    {
        [FieldOrder(0)]
        public uint FieldOffsetField1 { get; set; }

        [FieldOrder(1)]
        [FieldPosition(nameof(FieldOffsetField1))]
        public uint Field1 { get; set; }

        [FieldOrder(2)]
        public uint FieldOffsetField2 { get; set; }

        [FieldOrder(3)]
        [FieldPosition(nameof(FieldOffsetField2))]
        public uint Field2 { get; set; }

        [FieldOrder(4)]
        public uint FieldOffsetField3 { get; set; }

        [FieldOrder(5)]
        [FieldPosition(nameof(FieldOffsetField3), SeekOrigin.Current)]
        public uint Field3 { get; set; }
    }
}