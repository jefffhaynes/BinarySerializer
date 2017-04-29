namespace BinarySerialization.Test.Scale
{
    public class ScaledValueClass
    {
        [FieldScale(2)]
        [SerializeAs(SerializedType.Int1)]
        public double Value { get; set; }
    }
}
