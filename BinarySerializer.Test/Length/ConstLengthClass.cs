using BinarySerialization;

namespace BinarySerializer.Test.Length
{
    public class ConstLengthClass
    {
        [FieldLength(3)]
        public string Field { get; set; }
    }
}