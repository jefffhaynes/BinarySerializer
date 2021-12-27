namespace BinarySerialization.Test.Issues.Issue63;

class TestingClass
{
    [FieldOrder(0)]
    [FieldLength(2)]
    public byte[] Field1 { get; set; }

    [FieldOrder(1)]
    [FieldLength(6)]
    public byte[] Field2 { get; set; }
}
