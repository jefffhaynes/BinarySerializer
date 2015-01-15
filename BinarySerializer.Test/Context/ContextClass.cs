using BinarySerialization;

namespace BinarySerializer.Test.Context
{
    public class ContextClass
    {
        [SerializeWhen("SerializeCondtion", true, Mode = RelativeSourceMode.SerializationContext)]
        public int ContextConditionalField { get; set; }
    }
}
