namespace BinarySerialization.Test.Issues.Issue88
{
    public class ParentClass
    {
        [FieldOrder(0)]
        public int Value { get; set; }

        [FieldOrder(1)]
        public ChildClass Child { get; set; }
    }
}
