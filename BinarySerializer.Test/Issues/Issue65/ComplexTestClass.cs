namespace BinarySerialization.Test.Issues.Issue65
{
    public class ComplexTestClass
    {
        [FieldOrder(0)]
        public ComplexClass ComplexClass { get; set; }
    }
}