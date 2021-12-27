namespace BinarySerialization.Test.Scale;

public class ScaledValueClass
{
    [FieldScale(2)]
    [SerializeAs(SerializedType.Int4)]
    public double Value { get; set; }
}
