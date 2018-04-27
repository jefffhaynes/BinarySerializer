namespace BinarySerialization.Test.Unknown
{
    public class BindingAcrossUnknownBoundaryChildClass
    {
        [FieldLength(nameof(BindingAcrossUnknownBoundaryClass.SubfieldLength), 
            AncestorLevel = 2, RelativeSourceMode = RelativeSourceMode.FindAncestor)]
        public string Subfield { get; set; }
    }
}