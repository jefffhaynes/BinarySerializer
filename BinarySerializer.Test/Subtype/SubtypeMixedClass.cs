namespace BinarySerialization.Test.Subtype
{
    public class SubtypeMixedClass
    {
        [FieldOrder(0)]
        public int Key { get; set; }

        [FieldOrder(1)]
        [Subtype("Key", 3, typeof(SubSubclassC))]
        [SubtypeFactory("Key", typeof(SubtypeFactory))]
        public Superclass Value { get; set; }
    }
}
