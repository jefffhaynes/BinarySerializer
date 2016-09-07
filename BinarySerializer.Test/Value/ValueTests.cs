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
                    IntegerValue = 1,
                    Value = "hello world"
                }
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xbc68, actual.Crc);
        }

        [TestMethod]
        public void Crc32Test()
        {
            var expected = new FieldCrc32Class
            {
                Internal = new FieldCrcInternalClass
                {
                    Value = "hello world"
                }
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xfd11ac49, actual.Crc);
        }

        [TestMethod]
        public void Crc16StreamTest()
        {
            var expected = new StreamValueClass
            {
                Data = new MemoryStream(Enumerable.Repeat((byte) 'A', 100000).ToArray())
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xdef, actual.Crc);
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
                new SHA256Managed().ComputeHash(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(expected.Value)));

            Assert.IsTrue(expectedHash.SequenceEqual(actual.Hash));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EasyMistakeCrcTest()
        {
            Roundtrip(new EasyMistakeCrcClass());
        }
    }
}