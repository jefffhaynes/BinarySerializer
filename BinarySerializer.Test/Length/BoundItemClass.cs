namespace BinarySerialization.Test.Length
{
    public class BoundItemClass
    {
        [FieldLength("NameLength", RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorLevel = 3)]
        public string Name { get; set; }
    }
}