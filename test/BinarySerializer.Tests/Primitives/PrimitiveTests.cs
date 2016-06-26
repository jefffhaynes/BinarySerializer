using System;
using Xunit;

namespace BinarySerialization.Test.Primitives
{
        public class PrimitiveTests : TestBase
    {
        private void RoundtripPrimitive<T>(T value, long expectedLength) where T : new()
        {
            var actual = Roundtrip(value, expectedLength);
            Assert.Equal(value, actual);
        }

        [Fact]
        public void BoolTest()
        {
            RoundtripPrimitive(true, 1);
        }

        [Fact]
        public void ByteTest()
        {
            RoundtripPrimitive(byte.MaxValue, 1);
        }

        [Fact]
        public void SByteTest()
        {
            RoundtripPrimitive(sbyte.MaxValue, 1);
        }

        [Fact]
        public void CharTest()
        {
            RoundtripPrimitive(char.MaxValue, 2);
        }

        [Fact]
        public void ShortTest()
        {
            RoundtripPrimitive(short.MaxValue, 2);
        }

        [Fact]
        public void UShortTest()
        {
            RoundtripPrimitive(ushort.MaxValue, 2);
        }

        [Fact]
        public void Int32Test()
        {
            RoundtripPrimitive(Int32.MaxValue, 4);
        }

        [Fact]
        public void UInt32Test()
        {
            RoundtripPrimitive(UInt32.MaxValue, 4);
        }

        [Fact]
        public void Int64Test()
        {
            RoundtripPrimitive(Int64.MaxValue, 8);
        }

        [Fact]
        public void UInt64Test()
        {
            RoundtripPrimitive(UInt64.MaxValue, 8);
        }

        [Fact]
        public void SingleTest()
        {
            RoundtripPrimitive(Single.MaxValue, 4);
        }

        [Fact]
        public void DoubleTest()
        {
            RoundtripPrimitive(Double.MaxValue, 8);
        }

        [Fact]
        public void ByteArrayTest()
        {
            Roundtrip(new byte[3], new byte[3]);
        }

        [Fact]
        public void StringTest()
        {
            const string value = "hello";
            Roundtrip(value, System.Text.Encoding.UTF8.GetBytes(value));
        }


        [Fact]
        public void NullTerminatedStringTest()
        {
            var container = new ContainedStringClass {Value = "hello"};
            Roundtrip(container, container.Value.Length + 5);
        }
    }
}
