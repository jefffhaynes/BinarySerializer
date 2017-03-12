using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Primitives
{
    [TestClass]
    public class PrimitiveTests : TestBase
    {
        private void RoundtripPrimitive<T>(T value, long expectedLength) where T : new()
        {
            var actual = Roundtrip(value, expectedLength);
            Assert.AreEqual(value, actual);

            var actualBe = RoundtripBigEndian(value, expectedLength);
            Assert.AreEqual(value, actualBe);
        }

        [TestMethod]
        public void BoolTest()
        {
            RoundtripPrimitive(true, 1);
        }

        [TestMethod]
        public void ByteMaxTest()
        {
            RoundtripPrimitive(byte.MaxValue, sizeof(byte));
        }

        [TestMethod]
        public void ByteMinTest()
        {
            RoundtripPrimitive(byte.MinValue, sizeof(byte));
        }

        [TestMethod]
        public void SByteMaxTest()
        {
            RoundtripPrimitive(sbyte.MaxValue, sizeof(sbyte));
        }

        [TestMethod]
        public void SByteMinTest()
        {
            RoundtripPrimitive(sbyte.MinValue, sizeof(sbyte));
        }

        [TestMethod]
        public void CharMaxTest()
        {
            RoundtripPrimitive(char.MaxValue, sizeof(char));
        }

        [TestMethod]
        public void CharMinTest()
        {
            RoundtripPrimitive(char.MinValue, sizeof(char));
        }

        [TestMethod]
        public void ShortMaxTest()
        {
            RoundtripPrimitive(short.MaxValue, sizeof(short));
        }

        [TestMethod]
        public void ShortMinTest()
        {
            RoundtripPrimitive(short.MinValue, sizeof(short));
        }

        [TestMethod]
        public void UShortMaxTest()
        {
            RoundtripPrimitive(ushort.MaxValue, sizeof(ushort));
        }

        [TestMethod]
        public void UShortMinTest()
        {
            RoundtripPrimitive(ushort.MinValue, sizeof(ushort));
        }

        [TestMethod]
        public void Int32MaxTest()
        {
            RoundtripPrimitive(int.MaxValue, sizeof(int));
        }

        [TestMethod]
        public void Int32MinTest()
        {
            RoundtripPrimitive(int.MinValue, sizeof(int));
        }

        [TestMethod]
        public void UInt32MaxTest()
        {
            RoundtripPrimitive(uint.MaxValue, sizeof(uint));
        }

        [TestMethod]
        public void UInt32MinTest()
        {
            RoundtripPrimitive(uint.MinValue, sizeof(uint));
        }

        [TestMethod]
        public void Int64MaxTest()
        {
            RoundtripPrimitive(long.MaxValue, sizeof(long));
        }

        [TestMethod]
        public void Int64MinTest()
        {
            RoundtripPrimitive(long.MinValue, sizeof(long));
        }

        [TestMethod]
        public void UInt64MaxTest()
        {
            RoundtripPrimitive(ulong.MaxValue, sizeof(ulong));
        }

        [TestMethod]
        public void UInt64MinTest()
        {
            RoundtripPrimitive(ulong.MinValue, sizeof(ulong));
        }

        [TestMethod]
        public void SingleMaxTest()
        {
            RoundtripPrimitive(float.MaxValue, sizeof(float));
        }

        [TestMethod]
        public void SingleMinTest()
        {
            RoundtripPrimitive(float.MinValue, sizeof(float));
        }

        [TestMethod]
        public void SingleTest2()
        {
            RoundtripPrimitive(-48.651363f, sizeof(float));
        }

        [TestMethod]
        public void DoubleMaxTest()
        {
            RoundtripPrimitive(double.MaxValue, sizeof(double));
        }

        [TestMethod]
        public void DoubleMinTest()
        {
            RoundtripPrimitive(double.MinValue, sizeof(double));
        }

        [TestMethod]
        public void ByteArrayTest()
        {
            Roundtrip(new byte[3], new byte[3]);
        }

        [TestMethod]
        public void StringTest()
        {
            const string value = "hello";
            Roundtrip(value, System.Text.Encoding.UTF8.GetBytes(value + "\0"));
        }
        
        [TestMethod]
        public void NullTerminatedStringTest()
        {
            var container = new ContainedStringClass {Value = "hello"};
            Roundtrip(container, container.Value.Length + 5);
        }

        [TestMethod]
        public void PrimitiveBindingTest()
        {
            var expected = new PrimitiveBindingsClass
            {
                Value = "hello"
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Value.Length, actual.ByteLength);
            Assert.AreEqual(expected.Value.Length, actual.SByteLength);
            Assert.AreEqual(expected.Value.Length, actual.ShortLength);
            Assert.AreEqual(expected.Value.Length, actual.UShortLength);
            Assert.AreEqual(expected.Value.Length, actual.IntLength);
            Assert.AreEqual((uint)expected.Value.Length, actual.UIntLength);
            Assert.AreEqual(expected.Value.Length, actual.LongLength);
            Assert.AreEqual((ulong)expected.Value.Length, actual.ULongLength);
            Assert.AreEqual(expected.Value.Length, actual.FloatLength);
            Assert.AreEqual(expected.Value.Length, actual.DoubleLength);
            Assert.AreEqual(expected.Value.Length, actual.CharLength);
        }
    }
}