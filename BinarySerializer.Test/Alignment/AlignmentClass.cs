namespace BinarySerialization.Test.Alignment
{
    public class AlignmentClass
    {
        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length))]
        [FieldAlignment(4)]
        public string Value { get; set; }
    }
}
