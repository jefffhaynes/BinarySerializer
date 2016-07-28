namespace BinarySerialization.Test.Encoding
{
    public class EncodingClass
    {
#pragma warning disable 618
        [SerializeAs(SerializedType.NullTerminatedString, Encoding = "windows-1256")]
#pragma warning restore 618
        public string Name { get; set; }
    }
}