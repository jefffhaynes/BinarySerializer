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
        }

        [TestMethod]
        public void BoolTest()
        {
            RoundtripPrimitive(true, 1);
        }

        [TestMethod]
        public void ByteTest()
        {
            RoundtripPrimitive(byte.MaxValue, 1);
        }

        [TestMethod]
        public void SByteTest()
        {
            RoundtripPrimitive(sbyte.MaxValue, 1);
        }

        [TestMethod]
        public void CharTest()
        {
            RoundtripPrimitive(char.MaxValue, 2);
        }

        [TestMethod]
        public void ShortTest()
        {
            RoundtripPrimitive(short.MaxValue, 2);
        }

        [TestMethod]
        public void UShortTest()
        {
            RoundtripPrimitive(ushort.MaxValue, 2);
        }

        [TestMethod]
        public void Int32Test()
        {
            RoundtripPrimitive(int.MaxValue, 4);
        }

        [TestMethod]
        public void UInt32Test()
        {
            RoundtripPrimitive(uint.MaxValue, 4);
        }

        [TestMethod]
        public void Int64Test()
        {
            RoundtripPrimitive(long.MaxValue, 8);
        }

        [TestMethod]
        public void UInt64Test()
        {
            RoundtripPrimitive(ulong.MaxValue, 8);
        }

        [TestMethod]
        public void SingleTest()
        {
            RoundtripPrimitive(float.MaxValue, 4);
        }

        [TestMethod]
        public void DoubleTest()
        {
            RoundtripPrimitive(double.MaxValue, 8);
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
    }
}