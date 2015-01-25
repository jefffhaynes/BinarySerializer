using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Primitives
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
            RoundtripPrimitive(Int32.MaxValue, 4);
        }

        [TestMethod]
        public void UInt32Test()
        {
            RoundtripPrimitive(UInt32.MaxValue, 4);
        }

        [TestMethod]
        public void Int64Test()
        {
            RoundtripPrimitive(Int64.MaxValue, 8);
        }

        [TestMethod]
        public void UInt64Test()
        {
            RoundtripPrimitive(UInt64.MaxValue, 8);
        }

        [TestMethod]
        public void SingleTest()
        {
            RoundtripPrimitive(Single.MaxValue, 4);
        }

        [TestMethod]
        public void DoubleTest()
        {
            RoundtripPrimitive(Double.MaxValue, 8);
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
            Roundtrip(value, System.Text.Encoding.UTF8.GetBytes(value));
        }
    }
}
