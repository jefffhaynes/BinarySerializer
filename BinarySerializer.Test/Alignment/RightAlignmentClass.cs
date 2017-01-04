namespace BinarySerialization.Test.Alignment
{
    public class RightAlignmentClass
    {
        [FieldOrder(0)]
        public byte Header { get; set; }

        [FieldOrder(1)]
        [FieldAlignment(4, FieldAlignmentMode.RightOnly)]
        public byte Value { get; set; }

        [FieldOrder(2)]
        public byte Trailer { get; set; }
    }
}
