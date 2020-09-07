using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Streams
{
    [TestClass]
    public class StreamTests : TestBase
    {
        [TestMethod]
        public void StreamTest()
        {
            var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("StreamValue"));
            var expected = new StreamClass {Field = stream};
            var actual = Roundtrip(expected);
            Assert.AreEqual(stream.Length, actual.Field.Length);
        }

        [TestMethod]
        public void BoundedStreamSetLengthThrowsNotSupported()
        {
            Assert.ThrowsException<NotSupportedException>(() => new BoundedStream(new MemoryStream(), string.Empty).SetLength(0));
        }

        [TestMethod]
        public void BoundedStreamToStringIsName()
        {
            const string name = "Name";
            var stream = new BoundedStream(new MemoryStream(), name);
            Assert.AreEqual(name, stream.ToString());
        }
    }
}