namespace BinarySerialization.Test.Subtype
{
    public class SubtypeClass
    {
        [FieldOrder(0)]
        public SubclassType Subtype { get; set; }

        [FieldOrder(1)]
        [Subtype("Subtype", SubclassType.A, typeof(SubclassA))]
        [Subtype("Subtype", SubclassType.B, typeof(SubclassB))]
        [Subtype("Subtype", SubclassType.C, typeof(SubSubclassC))]
        public Superclass Field { get; set; }

        [FieldOrder(2)]
        public string SubtypeString { get; set; }

        [FieldOrder(3)]
        [Subtype("SubtypeString", "A", typeof(SubclassA))]
        [Subtype("SubtypeString", "B", typeof(SubclassB))]
        [Subtype("SubtypeString", "C", typeof(SubSubclassC))]
        public Superclass Field2 { get; set; }
    }
}