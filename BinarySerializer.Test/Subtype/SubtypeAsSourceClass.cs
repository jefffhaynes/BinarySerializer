namespace BinarySerialization.Test.Subtype
{
    public class SubtypeAsSourceClass
    {
        [FieldOrder(0)]
        public byte Selector { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(Selector), 42, typeof (SubclassA))]
        public Superclass Superclass { get; set; }

        [FieldOrder(2)]
        [FieldLength(nameof(Superclass) + "." + nameof(SubclassA.SomethingForClassA))]
        public string Name { get; set; }
    }
}