using BinarySerialization;

namespace BinarySerializer.Test.Length
{
    public class EmbeddedConstrainedCollectionClass
    {
        [FieldLength(10)]
        public EmbeddedConstrainedCollectionInnerClass Inner { get; set; }
    }
}
