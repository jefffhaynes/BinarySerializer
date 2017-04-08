namespace BinarySerialization.Test.Length
{
    public class PrimitiveArrayClass<TValue>
    {
        [FieldLength(5)]
        public TValue[] Array { get; set; }
    }
}
