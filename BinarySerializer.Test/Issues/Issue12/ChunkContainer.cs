namespace BinarySerialization.Test.Issues.Issue12
{
    public class ChunkContainer
    {
        public ChunkContainer()
        {
        }

        public ChunkContainer(Chunk chunk)
        {
            Chunk = chunk;
        }

        [FieldOrder(0)]
        [FieldLength(4)]
        public string TypeId { get; set; }

        [FieldOrder(1)]
        [SerializeAs(Endianness = BinarySerialization.Endianness.Big)]
        public int ChunkLength { get; set; }

        [FieldOrder(2)]
        [FieldLength("ChunkLength")]
        [Subtype("TypeId", ChunkType.Form, typeof(FormChunk))]
        [Subtype("TypeId", ChunkType.Cat, typeof(CatChunk))]
        [Subtype("TypeId", ChunkType.List, typeof(ListChunk))]
        [Subtype("TypeId", ChunkType.Refe, typeof(RefeChunk))]
        public Chunk Chunk { get; set; }

        [FieldOrder(3)]
        [SerializeWhen("ChunkLength", false, ConverterType = typeof(IsEvenConverter))]
        public byte Pad { get; set; }
    }
}
