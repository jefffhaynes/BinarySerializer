namespace BinarySerialization.Test.Issues.Issue147;

class ClassB
{
    [FieldOrder(1)]
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
#pragma warning disable CS0649
    public int XXX;
#pragma warning restore CS0649

    [FieldOrder(2)]
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
#pragma warning disable CS0649
    public int YYY;
#pragma warning restore CS0649
}
