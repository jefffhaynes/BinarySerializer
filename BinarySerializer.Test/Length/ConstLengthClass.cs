
namespace BinarySerialization.Test.Length
{
    public class ConstLengthClass
    {
        [FieldLength(3)]
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Field { get; set; }
    }
}