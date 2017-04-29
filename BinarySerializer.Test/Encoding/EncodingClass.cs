namespace BinarySerialization.Test.Encoding
{
    public class EncodingClass
    {
        [FieldEncoding("windows-1256")]
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Name { get; set; }
    }
}