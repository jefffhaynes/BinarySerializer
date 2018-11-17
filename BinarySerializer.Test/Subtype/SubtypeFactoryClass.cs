namespace BinarySerialization.Test.Subtype
{
    public class SubtypeFactoryClass
    {
        [FieldOrder(0)]
        public int Key { get; set; }

        [FieldOrder(1)]
        [SubtypeFactory("Key", typeof(SubtypeFactory))]
        public Superclass Value { get; set; }
    }
}
