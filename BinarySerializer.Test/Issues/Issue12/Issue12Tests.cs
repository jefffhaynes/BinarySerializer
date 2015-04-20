using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue12
{
    [TestClass]
    public class Issue12Tests : TestBase
    {
        [TestMethod]
        public void Roundtrip12()
        {
            var expected = new FormChunk
            {
                Chunks = new List<ChunkContainer>
                {
                    new ChunkContainer
                    {
                        Chunk = new CatChunk
                        {
                            Chunks = new List<ChunkContainer>
                            {
                                new ChunkContainer(new RefeChunk {SomeStuffInThisChunk = "hello"}),
                                new ChunkContainer(new RefeChunk {SomeStuffInThisChunk = "world"}),
                            }
                        }
                    }
                }
            };

            Roundtrip(expected);
        }
    }
}
