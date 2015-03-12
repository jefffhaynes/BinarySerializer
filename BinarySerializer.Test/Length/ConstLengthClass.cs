using BinarySerialization;

namespace BinarySerialization.Test.Length
{
    public class ConstLengthClass
    {
        [FieldLength(3)]
        public string Field { get; set; }
    }
}