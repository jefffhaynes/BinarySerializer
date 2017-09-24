using BinarySerialization;

namespace BinarySerializer.Editor.Test
{
    public class PngInternationalTextChunk : PngChunk
    {
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Keyword { get; set; }

        public byte CompressionFlag { get; set; }

        public byte CompressionMethod { get; set; }

        [SerializeAs(SerializedType.NullTerminatedString)]
        public string LanguageTag { get; set; }

        [SerializeAs(SerializedType.NullTerminatedString)]
        public string TranslatedKeyword { get; set; }

        public string Text { get; set; }
    }
}
