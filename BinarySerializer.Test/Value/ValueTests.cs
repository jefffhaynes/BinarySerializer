using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Value
{
    [TestClass]
    public class ValueTests : TestBase
    {
        [TestMethod]
        public void FieldValueTest()
        {
            var expected = new FieldValueClass {Value = 33};
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Value, actual.Value);
            Assert.AreEqual(actual.Value, actual.ValueCopy);
        }

        [TestMethod]
        public void Crc16Test()
        {
            var expected = new FieldCrc16Class
            {
                Internal = new FieldCrcInternalClass
                {
                    UshortValue = 1,
                    ByteValue = 2,
                    ArrayValue = new byte[] {0x3, 0x4},
                    Value = "hello world"
                }
            };

            var expectedData = new byte[]
            {
                0x10, 0x0, 0x0, 0x0,
                0x01, 0x00,
                0x02,
                0x03, 0x04,
                0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64,
                0x79, 0xcd
            };

            var actual = Roundtrip(expected, expectedData);
            Assert.AreEqual(0xcd79, actual.Crc);
        }

        [TestMethod]
        public void Crc32Test()
        {
            var expected = new FieldCrc32Class
            {
                Internal = new FieldCrcInternalClass
                {
                    UshortValue = 1,
                    ByteValue = 2,
                    ArrayValue = new byte[] { 0x3, 0x4 },
                    Value = "hello world"
                }
            };

            var expectedData = new byte[]
            {
                0x10, 0x0, 0x0, 0x0,
                0x01, 0x00,
                0x02,
                0x03, 0x04,
                0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64,
                0xdf, 0x4d, 0x34, 0xf8
            };

            var actual = Roundtrip(expected, expectedData);
            Assert.AreEqual(0xF8344DDF, actual.Crc);
        }

        [TestMethod]
        public void Crc16StreamTest()
        {
            var expected = new StreamValueClass
            {
                Data = new MemoryStream(Enumerable.Repeat((byte) 'A', 100000).ToArray())
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xdb9, actual.Crc);
        }

        [TestMethod]
        public void FieldValueExtensionTest()
        {
            var expected = new FieldSha256Class
            {
                Value = "hello world"
            };

            var actual = Roundtrip(expected);
            
            var expectedHash =
                SHA256.Create().ComputeHash(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(expected.Value)));

            Assert.IsTrue(expectedHash.SequenceEqual(actual.Hash));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EasyMistakeCrcTest()
        {
            Roundtrip(new EasyMistakeCrcClass());
        }

        [TestMethod]
        public void ChecksumTest()
        {
            var expected = new FieldChecksumClass
            {
                Value = "hello"
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(0xEC, actual.Checksum);
            Assert.AreEqual(0x14, actual.ModuloChecksum);
            Assert.AreEqual(0x62, actual.XorChecksum);
        }
    }
}