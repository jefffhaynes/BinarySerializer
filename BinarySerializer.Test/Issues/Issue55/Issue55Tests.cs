using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue55
{
    [TestClass]
    public class Issue55Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {

            var source = new ChunkContainer
            {
                ChunkType = "TEST",
                Chunk = new TestChunk
                {
                    Customs = new[]
                    {
                        new CustomSerializable
                        {
                            Value = 1
                        },
                        new CustomSerializable
                        {
                            Value = 2
                        }
                    }
                }
            };

            Roundtrip(source);

            var serializer = new BinarySerializer();
            var outputStream = new MemoryStream();
            serializer.Serialize(outputStream, source);

            outputStream.Seek(0, SeekOrigin.Begin);
            var inputStream = new LooksLikeANetworkStream(outputStream); //non-seekable stream

            var roundtrip = serializer.Deserialize<ChunkContainer>(inputStream);
            Assert.IsInstanceOfType<TestChunk>(roundtrip.Chunk);
            var sourceChunk = (TestChunk)source.Chunk;
            var testChunk = (TestChunk)roundtrip.Chunk;
            Assert.AreEqual(testChunk.Customs.Length, sourceChunk.Customs.Length);
            
            Assert.AreEqual(testChunk.Customs[0].Value, sourceChunk.Customs[0].Value);
            Assert.AreEqual(testChunk.Customs[1].Value, sourceChunk.Customs[1].Value);
        }
    }
}