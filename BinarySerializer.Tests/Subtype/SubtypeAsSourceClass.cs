namespace BinarySerialization.Test.Subtype
{
    public class SubtypeAsSourceClass
    {
        [FieldOrder(0)]
        public byte Selector { get; set; }

        [FieldOrder(1)]
        [Subtype("Selector", 42, typeof(SubclassA))]
        public Superclass Superclass { get; set; }

        [FieldOrder(2)]
        [FieldLength("Superclass.SomethingForClassA")]
        public string Name { get; set; }
    }
}
