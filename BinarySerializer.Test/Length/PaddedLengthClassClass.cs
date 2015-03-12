using BinarySerialization;

namespace BinarySerialization.Test.Length
{
    public class PaddedLengthClassClass
    {
        [FieldLength(20)]
        public PaddedLengthClassInnerClass InnerClass { get; set; }
    }
}
