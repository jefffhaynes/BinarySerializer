using BinarySerialization;

namespace BinarySerializer.Test.Length
{
    public class PaddedLengthClassClass
    {
        [FieldLength(20)]
        public PaddedLengthClassInnerClass InnerClass { get; set; }
    }
}
