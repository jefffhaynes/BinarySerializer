using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Endianness
{
    [TestClass]
    public class EndiannessTests
    {
        [TestMethod]
        public void TestSerializerEndianness()
        {
            var serializer = new BinarySerialization.BinarySerializer {Endianness = BinarySerialization.Endianness.Big};
            var expected = new EndiannessClass {Short = 1};

            var stream = new MemoryStream();
            serializer.Serialize(stream, expected);

            var data = stream.ToArray();

            Assert.AreEqual(0x1, data[1]);
        }
    }
}
