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
        public void ByteTest()
        {
            RoundtripPrimitive(byte.MaxValue, sizeof(byte));
        }

        [TestMethod]
        public void SByteTest()
        {
            RoundtripPrimitive(sbyte.MaxValue, sizeof(sbyte));
        }

        [TestMethod]
        public void CharTest()
        {
            RoundtripPrimitive(char.MaxValue, sizeof(char));
        }

        [TestMethod]
        public void ShortTest()
        {
            RoundtripPrimitive(short.MaxValue, sizeof(short));
        }

        [TestMethod]
        public void UShortTest()
        {
            RoundtripPrimitive(ushort.MaxValue, sizeof(ushort));
        }

        [TestMethod]
        public void Int32Test()
        {
            RoundtripPrimitive(int.MaxValue, sizeof(int));
        }

        [TestMethod]
        public void UInt32Test()
        {
            RoundtripPrimitive(uint.MaxValue, sizeof(uint));
        }

        [TestMethod]
        public void Int64Test()
        {
            RoundtripPrimitive(long.MaxValue, sizeof(long));
        }

        [TestMethod]
        public void UInt64Test()
        {
            RoundtripPrimitive(ulong.MaxValue, sizeof(ulong));
        }

        [TestMethod]
        public void SingleTest()
        {
            RoundtripPrimitive(float.MaxValue, sizeof(float));
        }

        [TestMethod]
        public void SingleTest2()
        {
            RoundtripPrimitive(-48.651363f, sizeof(float));
        }

        [TestMethod]
        public void DoubleTest()
        {
            RoundtripPrimitive(double.MaxValue, sizeof(double));
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