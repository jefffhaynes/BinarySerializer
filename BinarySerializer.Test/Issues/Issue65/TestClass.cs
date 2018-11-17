namespace BinarySerialization.Test.Issues.Issue65
{
    public class TestClass
    {
        [FieldCount(5)]
        public short[] ShortArray { get; set; }
    }
}
