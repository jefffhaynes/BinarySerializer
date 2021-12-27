namespace BinarySerialization.Test.Context;

public class ContextClass
{
    [SerializeWhen(nameof(Context.SerializeCondtion), true, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
    public int ContextConditionalField { get; set; }
}
