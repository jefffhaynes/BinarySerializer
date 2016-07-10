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
        [Subtype("TypeId", "FORM", typeof (FormChunk))]
        [Subtype("TypeId", "CAT ", typeof (CatChunk))]
        [Subtype("TypeId", "LIST", typeof (ListChunk))]
        [Subtype("TypeId", "REFE", typeof (RefeChunk))]
        [Subtype("TypeId", "DESC", typeof (DescChunk))]
        [Subtype("TypeId", "BEER", typeof (BeerChunk))]
        [Subtype("TypeId", "SNAX", typeof (SnaxChunk))]
        [Subtype("TypeId", "PARM", typeof (ParmChunk))]
        [Subtype("TypeId", "BODY", typeof (BodyChunk))]
        public Chunk Chunk { get; set; }

        [FieldOrder(3)]
        [SerializeWhen("ChunkLength", false, ConverterType = typeof (IsEvenConverter))]
        public byte Pad { get; set; }
    }
}