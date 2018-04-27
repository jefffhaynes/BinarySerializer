namespace BinarySerialization.Test.Length
{
    public class BoundItemClass
    {
        [FieldLength(nameof(BoundItemContainerClass.NameLength), 
            RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorLevel = 3)]
        public string Name { get; set; }
    }
}