namespace BinarySerialization.Test.Length
{
    public class AncestorBindingCollectionItemClass
    {
        [FieldLength("ItemLength", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorLevel = 3)]
        public string Value { get; set; }
    }
}
