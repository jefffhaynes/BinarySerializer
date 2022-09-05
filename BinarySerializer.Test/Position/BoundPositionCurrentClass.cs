using System.IO;

namespace BinarySerialization.Test.Position
{
    public class BoundPositionCurrentClass
    {
        [FieldOrder(0)]
        public uint FieldOffsetField { get; set; }

        [FieldOrder(1)]
        [FieldPosition(nameof(FieldOffsetField), SeekOrigin.Current)]
        public uint Field { get; set; }

        [FieldOrder(2)]
        public uint LastInt { get; set; }
    }
}