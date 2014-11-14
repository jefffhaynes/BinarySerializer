using BinarySerialization;

namespace BinarySerializer.Test.Subtype
{
    public class SubtypeClass
    {
        public SubclassType Subtype { get; set; }

        [Subtype("Subtype", SubclassType.A, typeof(SubclassA))]
        [Subtype("Subtype", SubclassType.B, typeof(SubclassB))]
        [Subtype("Subtype", SubclassType.C, typeof(SubSubclassC))]
        public Superclass Field { get; set; }
    }
}