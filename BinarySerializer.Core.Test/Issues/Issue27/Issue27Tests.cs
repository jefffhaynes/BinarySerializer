using System.IO;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue27
{
    public class Issue27Tests
    {
        [Fact]
        //[ExpectedException(typeof(EndOfStreamException))]
        public void TestPrematureStreamTermination()
        {
            var serializer = new BinarySerializer { Endianness = BinarySerialization.Endianness.Little };
            var inBytes = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x40, 0x34 };

            using (var stream = new MemoryStream(inBytes))
            {

                var e = Record.Exception(() => serializer.Deserialize<LoadCarrierData>(stream));

               // Assert.Null(e);
                Assert.IsType<EndOfStreamException>(e);
                //Assert.Equal(null, e);//, "Deserialization done with invalid Stream.");
            }
        }
    }
}
