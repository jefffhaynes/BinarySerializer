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
            var expected = new ChunkContainer
            {
                Chunk = new FormChunk
                {
                    TypeId = "PTCH",
                    Chunks = new List<ChunkContainer>
                    {
                        new ChunkContainer
                        {
                            Chunk = new CatChunk
                            {
                                TypeId = "REFS",
                                Chunks = new List<ChunkContainer>
                                {
                                    new ChunkContainer(new RefeChunk {SomeStuffInThisChunk = "hello"}),
                                    new ChunkContainer(new RefeChunk {SomeStuffInThisChunk = "worlds"}),
                                }
                            }
                        }
                    }
                }
            };

            Roundtrip(expected, new[] 
            {
                (byte)'F', (byte)'O', (byte)'R', (byte)'M', 
                (byte)0, (byte)0, (byte)0, (byte)44, 
                (byte)'P', (byte)'T', (byte)'C', (byte)'H', 
                (byte)'C', (byte)'A', (byte)'T', (byte)' ',
                (byte)0, (byte)0, (byte)0, (byte)32, 
                (byte)'R', (byte)'E', (byte)'F', (byte)'S',
                (byte)'R', (byte)'E', (byte)'F', (byte)'E',
                (byte)0, (byte)0, (byte)0, (byte)5, 
                (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o',
                (byte)0,
                (byte)'R', (byte)'E', (byte)'F', (byte)'E',
                (byte)0, (byte)0, (byte)0, (byte)6,
                (byte)'w', (byte)'o', (byte)'r', (byte)'l', (byte)'d', (byte)'s' });
        }
    }
}
