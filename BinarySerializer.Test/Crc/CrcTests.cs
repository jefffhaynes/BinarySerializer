using System.Text;
using System.Linq;
using BinarySerialization;
using BinarySerialization.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Crc
{
    public class Payload
    {
        public int Length { get; set; }

        [FieldLength("Length")]
        public byte[] Data { get; set; }

        [FieldValue("Data", ConverterType = typeof(Crc32Converter))]
        public byte[] Crc { get; set; }
    }

    [TestClass]
    public class CrcTests : TestBase
    {
        [TestMethod]
        public void CrcTest()
        {
            var expected = new byte[] {0x52, 0x9e, 0xd6, 0x8b};
            var initial = new Payload {Data = Encoding.UTF8.GetBytes("Hello world")};
            var actual = Roundtrip(initial);
            Assert.IsTrue(expected.SequenceEqual(actual.Crc));
        }
    }
}
