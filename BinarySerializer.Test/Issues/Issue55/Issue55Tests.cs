namespace BinarySerialization.Test.Issues.Issue55;

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
                                                                     //var inputStream = outputStream; //successful

        var roundtrip = serializer.Deserialize<ChunkContainer>(inputStream);
        //Assert.That(roundtrip.Chunk, Is.InstanceOf<TestChunk>());

        //var sourceChunk = (TestChunk)source.Chunk;
        //var testChunk = (TestChunk)roundtrip.Chunk;
        //Assert.That(testChunk.Customs.Length, Is.EqualTo(sourceChunk.Customs.Length));
        //Assert.That(testChunk.Customs.ElementAt(0).Value, Is.EqualTo(sourceChunk.Customs.ElementAt(0).Value));
        //Assert.That(testChunk.Customs.ElementAt(1).Value, Is.EqualTo(sourceChunk.Customs.ElementAt(1).Value));
    }
}
