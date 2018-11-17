namespace BinarySerialization.Test.Count
{
    public class PrimitiveArrayConstClass<TValue>
    {
        [FieldCount(5)]
        public TValue[] Array { get; set; }
    }
}
