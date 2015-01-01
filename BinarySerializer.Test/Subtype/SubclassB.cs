using BinarySerialization;

namespace BinarySerializer.Test.Subtype
{
    public class SubclassB : Superclass
    {
        [FieldOrder(0)]
        public int SomethingForClassB { get; set; }
    }
}