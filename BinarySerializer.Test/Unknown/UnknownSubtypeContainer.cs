namespace BinarySerialization.Test.Unknown
{
    public class UnknownSubtypeContainer
    {
        [FieldOrder(0)]
        public int Type { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(Type), 1, typeof(ClassA))]
        [Subtype(nameof(Type), 2, typeof(ClassB))]
        public object Unknown { get; set; }
    }

    public class ClassA
    {
        public int Value { get; set; }
    }

    public class ClassB
    {
        public string Value { get; set; }
    }
}
