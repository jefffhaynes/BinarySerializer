namespace BinarySerialization.Test.Issues.Issue88
{
    public class ChildClass
    {
        [FieldValue("Value", AncestorLevel = 2, RelativeSourceMode = RelativeSourceMode.FindAncestor)]
        public int Value { get; set; }
    }
}
