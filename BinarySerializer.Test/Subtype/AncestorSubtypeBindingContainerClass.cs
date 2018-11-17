namespace BinarySerialization.Test.Subtype
{
    public class AncestorSubtypeBindingContainerClass
    {
        [FieldOrder(0)]
        public int Selector { get; set; }

        [FieldOrder(1)]
        [Subtype("Selector", 1, typeof (AncestorSubtypeBindingClass))]
        public AncestorSubtypeBindingBaseClass AncestorSubtypeBindingClass { get; set; }
    }
}