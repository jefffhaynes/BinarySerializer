using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace BinarySerialization.Test.Value
{
        public class ValueTests : TestBase
    {
        [Fact]
        public void FieldValueTest()
        {
            var expected = new FieldValueClass {Value = 33};
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Value, actual.Value);
            Assert.Equal(actual.Value, actual.ValueCopy);
        }

        [Fact]
        public void Crc16Test()
        {
            var expected = new FieldCrc16Class
            {
                Internal = new FieldCrcInternalClass
                {
                    Value = "hello world"
                }
            };

            var actual = Roundtrip(expected);
            Assert.Equal(0xefeb, actual.Crc);
        }

        [Fact]
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
            Assert.Equal(0xfd11ac49, actual.Crc);
        }

        [Fact]
        public void Crc16StreamTest()
        {
            var expected = new StreamValueClass
            {
                Data = new MemoryStream(Enumerable.Repeat((byte)'A', 100000).ToArray())
            };

            var actual = Roundtrip(expected);
            Assert.Equal(0xdef, actual.Crc);
        }

        [Fact]
        public void FieldValueExtensionTest()
        {
            Assert.True(false, "FieldValueExtensionTest Not Yet Implemented");
            //var expected = new FieldSha256Class
            //{
            //    Value = "hello world"
            //};

            //var actual = Roundtrip(expected);

            //var expectedHash =
            //    (new SHA256Managed()).ComputeHash(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(expected.Value)));

            //Assert.True(expectedHash.SequenceEqual(actual.Hash));
        }
    }
}
