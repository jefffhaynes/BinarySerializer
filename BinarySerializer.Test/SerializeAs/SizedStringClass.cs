namespace BinarySerialization.Test.SerializeAs;

public class SizedStringClass<TValue>
{
    [SerializeAs(SerializedType.SizedString)]
    [FieldLength(2)]
    public TValue Value { get; set; }
}
