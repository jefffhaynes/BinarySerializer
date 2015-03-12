using BinarySerialization;

namespace BinarySerialization.Test.SerializeAs
{
    public class LengthPrefixedStringClass
    {
        [SerializeAs(SerializedType.LengthPrefixedString)]
        public string Value { get; set; }
    }
}
