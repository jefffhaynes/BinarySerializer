namespace BinarySerialization.Test.Length
{
    public class InterfaceAncestoryBindingClass
    {
        [FieldLength("Length", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(ILengthSource))]
        public string Value { get; set; }
    }
}
