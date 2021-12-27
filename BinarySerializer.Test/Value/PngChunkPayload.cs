namespace BinarySerialization.Test.Value;

public class PngChunkPayload
{
    [FieldOrder(0)]
    [FieldLength(4)]
    public string ChunkType { get; set; }

    [FieldOrder(1)]
    [FieldLength(nameof(Length), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorLevel = 2)]
    [Subtype(nameof(ChunkType), "IHDR", typeof(PngImageHeaderChunk))]
    [Subtype(nameof(ChunkType), "PLTE", typeof(PngPaletteChunk))]
    [Subtype(nameof(ChunkType), "IDAT", typeof(PngImageDataChunk))]
    [SubtypeDefault(typeof(PngUnknownChunk))]
    public PngChunk Chunk { get; set; }
}
