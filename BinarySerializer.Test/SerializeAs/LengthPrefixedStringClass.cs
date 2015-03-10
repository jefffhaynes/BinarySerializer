using BinarySerialization;

namespace BinarySerializer.Test.SerializeAs
{
    public class LengthPrefixedStringClass
    {
        [SerializeAs(SerializedType.LengthPrefixedString)]
        public string Value { get; set; }
    }
}
