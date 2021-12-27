namespace BinarySerialization.Test.Issues.Issue147;

class Parent
{
#pragma warning disable CS0649
    [FieldOrder(1)] public ClassA config;
#pragma warning restore CS0649

    [FieldOrder(2)]
    [FieldCount("config.count", RelativeSourceMode = RelativeSourceMode.FindAncestor,
        AncestorType = typeof(ClassA))]
#pragma warning disable CS0649
    public List<ClassB> somelist;
#pragma warning restore CS0649
}
