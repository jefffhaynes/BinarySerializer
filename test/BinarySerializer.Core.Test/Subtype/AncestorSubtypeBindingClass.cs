namespace BinarySerialization.Test.Subtype
{
    public class AncestorSubtypeBindingClass : AncestorSubtypeBindingBaseClass
    {
        [FieldOrder(0)]
        public int ValueLength { get; set; }

        [FieldOrder(1)]
        public AncestorSubtypeBindingInnerClass InnerClass { get; set; }
    }
}