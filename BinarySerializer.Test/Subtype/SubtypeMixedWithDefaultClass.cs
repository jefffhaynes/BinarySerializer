namespace BinarySerialization.Test.Subtype
{
    public class SubtypeMixedWithDefaultClass
    {
        [FieldOrder(0)]
        public byte Key { get; set; }

        [FieldOrder(1)]
        [Subtype("Key", 3, typeof(SubSubclassC))]
        [SubtypeFactory("Key", typeof(SubtypeFactory))]
        [SubtypeDefault(typeof(DefaultSubtypeClass))]
        public Superclass Value { get; set; }
    }
}
