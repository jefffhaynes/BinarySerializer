using BinarySerialization;

namespace BinarySerializer.Editor.Test
{
    public class PngTextChunk : PngChunk
    {
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Keyword { get; set; }

        [SerializeAs(SerializedType.SizedString)]
        public string Value { get; set; }
    }
}
