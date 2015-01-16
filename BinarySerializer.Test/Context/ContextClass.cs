using BinarySerialization;

namespace BinarySerializer.Test.Context
{
    public class ContextClass
    {
        [SerializeWhen("SerializeCondtion", true, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public int ContextConditionalField { get; set; }
    }
}
