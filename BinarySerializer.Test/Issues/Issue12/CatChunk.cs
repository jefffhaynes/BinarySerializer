using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue12
{
    public class CatChunk : Chunk
    {
        public List<ChunkContainer> Chunks { get; set; } 
    }
}
