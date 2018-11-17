namespace BinarySerialization.Test.Issues.Issue55
{
    public class ChunkContainer
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        public string ChunkType { get; set; }

        [FieldOrder(2)]
        [FieldLength("Length")]
        [Subtype("ChunkType", "IHDR", typeof(ImageHeaderChunk))]
        [Subtype("ChunkType", "PLTE", typeof(PaletteChunk))]
        [Subtype("ChunkType", "IDAT", typeof(ImageDataChunk))]
        [Subtype("ChunkType", "TEST", typeof(TestChunk))]
        public Chunk Chunk { get; set; }

        [FieldOrder(3)]
        public int Crc { get; set; }
    }
}