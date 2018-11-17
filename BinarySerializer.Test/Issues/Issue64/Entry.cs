namespace BinarySerialization.Test.Issues.Issue64
{
    public class Entry
    {
        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldAlignment(5, FieldAlignmentMode.LeftOnly)]
        [FieldLength("Length")]
        public string Value { get; set; }
    }
}