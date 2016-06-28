using System.IO;
using Xunit;

namespace BinarySerialization.Test.Endianness
{
        public class EndiannessTests
    {
        [Fact]
        public void TestSerializerEndianness()
        {
            var serializer = new BinarySerialization.BinarySerializer {Endianness = BinarySerialization.Endianness.Big};
            var expected = new EndiannessClass {Short = 1};

            var stream = new MemoryStream();
            serializer.Serialize(stream, expected);

            var data = stream.ToArray();

            Assert.Equal(0x1, data[1]);
        }
    }
}
