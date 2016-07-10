namespace BinarySerialization.Test.Offset
{
    public class ConstOffsetClass
    {
        [FieldOffset(100)]
        public string Field { get; set; }
    }
}