using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Value
{
    [TestClass]
    public class ValueTests : TestBase
    {
        [TestMethod]
        public void TestCrc16()
        {
            var expected = new ValueClass
            {
                Internal = new ValueInternalClass
                {
                    Value = "hello world"
                }
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xefeb, actual.Crc);
        }

        [TestMethod]
        public void TestCrc16Again()
        {
            var expected = new ValueClass
            {
                Internal = new ValueInternalClass
                {
                    Value = "hello world again"
                }
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0x8733, actual.Crc);
        }


        [TestMethod]
        public void TestCrc16Stream()
        {
            var expected = new StreamValueClass
            {
                Data = new MemoryStream(Enumerable.Repeat((byte)'A', 100000).ToArray())
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xdb9, actual.Crc);
        }
    }
}
