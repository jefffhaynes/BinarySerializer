using BinarySerialization;

namespace BinarySerializer.Test.Length
{
    public class BoundLengthClass<T>
    {
        public BoundLengthClass()
        {
            TrailingData = "trailing data";
        }

        public ushort FieldLengthField { get; set; }

        [FieldLength("FieldLengthField")]
        public T Field { get; set; }

        public string TrailingData { get; set; }
    }
}