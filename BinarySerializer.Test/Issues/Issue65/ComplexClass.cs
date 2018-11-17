namespace BinarySerialization.Test.Issues.Issue65
{
    public class ComplexClass
    {
        [FieldOrder(0)]
        [FieldLength(5)]
        public string Text { get; set; }
    }
}