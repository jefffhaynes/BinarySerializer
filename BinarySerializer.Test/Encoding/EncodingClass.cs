namespace BinarySerialization.Test.Encoding
{
    public class EncodingClass
    {
        [SerializeAs(SerializedType.NullTerminatedString, Encoding = "windows-1256")]
        public string Name { get; set; }
    }
}