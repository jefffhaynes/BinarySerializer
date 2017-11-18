using Xunit;

namespace BinarySerialization.Test.Primitives
{
    
    public class PrimitiveTests : TestBase
    {
        private void RoundtripPrimitive<T>(T value, long expectedLength) where T : new()
        {
            var actual = Roundtrip(value, expectedLength);
            Assert.Equal(value, actual);

            var actualBe = RoundtripBigEndian(value, expectedLength);
            Assert.Equal(value, actualBe);
        }

        [Fact]
        public void BoolTest()
        {
            RoundtripPrimitive(true, 1);
        }

        [Fact]
        public void ByteMaxTest()
        {
            RoundtripPrimitive(byte.MaxValue, sizeof(byte));
        }

        [Fact]
        public void ByteMinTest()
        {
            RoundtripPrimitive(byte.MinValue, sizeof(byte));
        }

        [Fact]
        public void SByteMaxTest()
        {
            RoundtripPrimitive(sbyte.MaxValue, sizeof(sbyte));
        }

        [Fact]
        public void SByteMinTest()
        {
            RoundtripPrimitive(sbyte.MinValue, sizeof(sbyte));
        }

        [Fact]
        public void CharMaxTest()
        {
            RoundtripPrimitive(char.MaxValue, sizeof(char));
        }

        [Fact]
        public void CharMinTest()
        {
            RoundtripPrimitive(char.MinValue, sizeof(char));
        }

        [Fact]
        public void ShortMaxTest()
        {
            RoundtripPrimitive(short.MaxValue, sizeof(short));
        }

        [Fact]
        public void ShortMinTest()
        {
            RoundtripPrimitive(short.MinValue, sizeof(short));
        }

        [Fact]
        public void UShortMaxTest()
        {
            RoundtripPrimitive(ushort.MaxValue, sizeof(ushort));
        }

        [Fact]
        public void UShortMinTest()
        {
            RoundtripPrimitive(ushort.MinValue, sizeof(ushort));
        }

        [Fact]
        public void Int32MaxTest()
        {
            RoundtripPrimitive(int.MaxValue, sizeof(int));
        }

        [Fact]
        public void Int32MinTest()
        {
            RoundtripPrimitive(int.MinValue, sizeof(int));
        }

        [Fact]
        public void UInt32MaxTest()
        {
            RoundtripPrimitive(uint.MaxValue, sizeof(uint));
        }

        [Fact]
        public void UInt32MinTest()
        {
            RoundtripPrimitive(uint.MinValue, sizeof(uint));
        }

        [Fact]
        public void Int64MaxTest()
        {
            RoundtripPrimitive(long.MaxValue, sizeof(long));
        }

        [Fact]
        public void Int64MinTest()
        {
            RoundtripPrimitive(long.MinValue, sizeof(long));
        }

        [Fact]
        public void UInt64MaxTest()
        {
            RoundtripPrimitive(ulong.MaxValue, sizeof(ulong));
        }

        [Fact]
        public void UInt64MinTest()
        {
            RoundtripPrimitive(ulong.MinValue, sizeof(ulong));
        }

        [Fact]
        public void SingleMaxTest()
        {
            RoundtripPrimitive(float.MaxValue, sizeof(float));
        }

        [Fact]
        public void SingleMinTest()
        {
            RoundtripPrimitive(float.MinValue, sizeof(float));
        }

        [Fact]
        public void SingleTest2()
        {
            RoundtripPrimitive(-48.651363f, sizeof(float));
        }

        [Fact]
        public void DoubleMaxTest()
        {
            RoundtripPrimitive(double.MaxValue, sizeof(double));
        }

        [Fact]
        public void DoubleMinTest()
        {
            RoundtripPrimitive(double.MinValue, sizeof(double));
        }

        [Fact]
        public void DoubleNanTest()
        {
            RoundtripPrimitive(double.NaN, sizeof(double));
        }

        [Fact]
        public void DoublePositiveInfinityTest()
        {
            RoundtripPrimitive(double.PositiveInfinity, sizeof(double));
        }

        [Fact]
        public void DoubleNegativeInfinityTest()
        {
            RoundtripPrimitive(double.NegativeInfinity, sizeof(double));
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
            Roundtrip(value, System.Text.Encoding.UTF8.GetBytes(value + "\0"));
        }
        
        [Fact]
        public void NullTerminatedStringTest()
        {
            var container = new ContainedStringClass {Value = "hello"};
            Roundtrip(container, container.Value.Length + 5);
        }

        [Fact]
        public void PrimitiveBindingTest()
        {
            var expected = new PrimitiveBindingsClass
            {
                Value = "hello"
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.Value.Length, actual.ByteLength);
            Assert.Equal(expected.Value.Length, actual.SByteLength);
            Assert.Equal(expected.Value.Length, actual.ShortLength);
            Assert.Equal(expected.Value.Length, actual.UShortLength);
            Assert.Equal(expected.Value.Length, actual.IntLength);
            Assert.Equal((uint)expected.Value.Length, actual.UIntLength);
            Assert.Equal(expected.Value.Length, actual.LongLength);
            Assert.Equal((ulong)expected.Value.Length, actual.ULongLength);
            Assert.Equal(expected.Value.Length, actual.FloatLength);
            Assert.Equal(expected.Value.Length, actual.DoubleLength);
            Assert.Equal(expected.Value.Length, actual.CharLength);
        }
    }
}