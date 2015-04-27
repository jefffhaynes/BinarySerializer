namespace BinarySerialization.Test.Subtype
{
    public class AncestorSubtypeBindingInnerClass
    {
        [FieldLength("ValueLength", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(AncestorSubtypeBindingClass))]
        public string Value { get; set; }
    }
}