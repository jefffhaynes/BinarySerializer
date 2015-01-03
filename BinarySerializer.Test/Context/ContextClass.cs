using BinarySerialization;

namespace BinarySerializer.Test.Context
{
    public class ContextClass
    {
        [SerializeWhen("SerializeCondtion", true, AncestorType = typeof(Context), Mode = RelativeSourceMode.FindAncestor)]
        public int ContextConditionalField { get; set; }
    }
}
