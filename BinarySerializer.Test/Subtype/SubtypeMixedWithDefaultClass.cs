namespace BinarySerialization.Test.Subtype
{
    public class SubtypeMixedWithDefaultClass
    {
        [FieldOrder(0)]
        public byte Key { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(Key), 3, typeof(SubSubclassC))]
        [SubtypeFactory(nameof(Key), typeof(SubtypeFactory))]
        [SubtypeDefault(typeof(DefaultSubtypeClass))]
        public Superclass Value { get; set; }
    }
}
