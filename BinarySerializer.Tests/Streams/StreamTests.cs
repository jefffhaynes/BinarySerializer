using System.IO;
using Xunit;

namespace BinarySerialization.Test.Streams
{
        public class StreamTests : TestBase
    {
        [Fact]
        public void StreamTest()
        {
            var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("StreamValue"));
            var expected = new StreamClass {Field = stream};
            var actual = Roundtrip(expected);
            Assert.Equal(stream.Length, actual.Field.Length);
        }
    }
}
