using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue12
{
    public class FormChunk : Chunk
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public string TypeId { get; set; }

        [FieldOrder(1)]
        public List<ChunkContainer> Chunks { get; set; }
    }
}