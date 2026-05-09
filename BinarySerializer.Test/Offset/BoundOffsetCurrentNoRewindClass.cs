using System.IO;

namespace BinarySerialization.Test.Offset
{
    public class BoundOffsetCurrentNoRewindClass
    {
    [FieldOrder(0)]
    public uint FieldOffsetField { get; set; }

    [FieldOrder(1)]
    [FieldOffset(nameof(FieldOffsetField), SeekOrigin.Current, false)]
    public uint Field { get; set; }

    [FieldOrder(2)]
    public uint LastUInt { get; set; }
}
}