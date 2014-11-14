using BinarySerialization;

namespace BinarySerializer.Test.Subtype
{
    public class IncompleteSubtypeClass
    {
        public SubclassType Subtype { get; set; }

        [Subtype("Subtype", SubclassType.A, typeof(SubclassA))]
        public Superclass Field { get; set; }
    }
}