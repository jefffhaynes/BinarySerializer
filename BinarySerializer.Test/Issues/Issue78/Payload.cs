namespace BinarySerialization.Test.Issues.Issue78
{
    public class Payload
    {
        [FieldOrder(1)]
        public int Number { get; set; }

        [FieldOrder(2)]
        public string String { get; set; }
    }
}