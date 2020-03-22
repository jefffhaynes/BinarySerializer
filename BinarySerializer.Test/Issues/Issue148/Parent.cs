namespace BinarySerialization.Test.Issues.Issue148
{
    public class Parent
    {
        [FieldOrder(0)]
        public Class1 Class1 { get; set; }

        [FieldOrder(1)]
        public Class2 Class2 { get; set; }
    }
}