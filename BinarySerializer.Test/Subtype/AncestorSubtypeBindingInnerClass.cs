namespace BinarySerialization.Test.Subtype
{
    public class AncestorSubtypeBindingInnerClass
    {
        [FieldOrder(0)]
        [FieldLength("ValueLength", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(AncestorSubtypeBindingClass))]
        public string Value { get; set; }

        [FieldOrder(1)]
        public int Trailer { get; set; }
    }
}