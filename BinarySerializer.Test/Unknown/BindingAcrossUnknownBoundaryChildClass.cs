using BinarySerialization;

namespace BinarySerializer.Test.Unknown
{
    public class BindingAcrossUnknownBoundaryChildClass
    {
        [FieldLength("SubfieldLength", AncestorLevel = 2, RelativeSourceMode = RelativeSourceMode.FindAncestor)]
        public string Subfield { get; set; }
    }
}
