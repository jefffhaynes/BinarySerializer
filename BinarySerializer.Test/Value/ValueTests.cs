using System;
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
            Assert.Equal(0xcd79, actual.Crc);
        }

        [Fact]
        public void CrcTestOneWay()
        {
            var expected = new FieldCrc16OneWayClass
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
                0x00, 0x00
            };

#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected, expectedData));
#else
            Assert.Throws<InvalidOperationException>(() =>  Roundtrip(expected, expectedData));
#endif
        }

        [Fact]
        public void CrcTestOneWayToSource()
        {
            var expected = new FieldCrc16OneWayToSourceClass
            {
                Internal = new FieldCrcInternalClass
                {
                    UshortValue = 1,
                    ByteValue = 2,
                    ArrayValue = new byte[] { 0x3, 0x4 },
                    Value = "hello world"
                }
            };

            var data = new byte[]
            {
                0x10, 0x0, 0x0, 0x0,
                0x01, 0x00,
                0x02,
                0x03, 0x04,
                0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64,
                0x00, 0x00
            };

            var actual = Deserialize<FieldCrc16OneWayToSourceClass>(data);

            Assert.Equal(expected.Internal.Value, actual.Internal.Value);
        }

        [Fact]
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
            Assert.Equal(0xF8344DDF, actual.Crc);
        }

        [Fact]
        public void Crc16StreamTest()
        {
            var expected = new StreamValueClass
            {
                Data = new MemoryStream(Enumerable.Repeat((byte) 'A', 100000).ToArray())
            };

            var actual = Roundtrip(expected);
            Assert.Equal(0xdb9, actual.Crc);
        }

        [Fact]
        public void FieldValueExtensionTest()
        {
            var expected = new FieldSha256Class
            {
                Value = "hello world"
            };

            var actual = Roundtrip(expected);
            
            var expectedHash =
                SHA256.Create().ComputeHash(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(expected.Value)));

            Assert.True(expectedHash.SequenceEqual(actual.Hash));
        }

        [Fact]
        public void EasyMistakeCrcTest()
        {
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(new EasyMistakeCrcClass()));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new EasyMistakeCrcClass()));
#endif
        }

        [Fact]
        public void ChecksumTest()
        {
            var expected = new FieldChecksumClass
            {
                Value = "hello"
            };

            var actual = Roundtrip(expected);

            Assert.Equal(0xEC, actual.Checksum);
            Assert.Equal(0x14, actual.ModuloChecksum);
            Assert.Equal(0x62, actual.XorChecksum);
        }

        [Fact]
        public void MultiValueFieldTest()
        {
            var expected = new FieldCrc16MultiFieldClass {Value1 = 0x1, Value2 = 0x0201, Value3 = 0x2};
            var actual = Roundtrip(expected);

            Assert.Equal(actual.Crc2, actual.Crc);
        }

        [Fact]
        public void NestedCrcTest()
        {
            var expected = new NestedCrcClass {Value = "hello"};
            var actual = Roundtrip(expected);
            Assert.Equal(0xd26e, actual.Crc);
        }

        [Fact]
        public void OuterCrcTest()
        {
            var value = new OuterCrcClass {NestedCrc = new NestedCrcClass {Value = "hello"}};
            var actual = Roundtrip(value);
            Assert.Equal(0xd26e, actual.NestedCrc.Crc);
            Assert.Equal(0x91f8, actual.Crc);
        }
    }
}