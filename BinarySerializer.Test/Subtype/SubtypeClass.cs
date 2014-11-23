using BinarySerialization;

namespace BinarySerializer.Test.Subtype
{
    public class SubtypeClass
    {
        public SubtypeClass()
        {
            SubtypeString = "B";
        }

        public SubclassType Subtype { get; set; }

        [Subtype("Subtype", SubclassType.A, typeof(SubclassA))]
        [Subtype("Subtype", SubclassType.B, typeof(SubclassB))]
        [Subtype("Subtype", SubclassType.C, typeof(SubSubclassC))]
        public Superclass Field { get; set; }

        public string SubtypeString { get; set; }

        [Subtype("SubtypeString", "A", typeof(SubclassA))]
        [Subtype("SubtypeString", "B", typeof(SubclassB))]
        [Subtype("SubtypeString", "C", typeof(SubSubclassC))]
        public Superclass Field2 { get; set; }
    }
}