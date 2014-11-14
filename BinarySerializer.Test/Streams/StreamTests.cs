using System.IO;
using System.Text;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Streams
{
    public class StreamClass
    {
        public StreamClass()
        {
            TrailingData = "blah blah";
        }

        public int FieldLength { get; set; }

        [FieldLength("FieldLength")]
        public Stream Field { get; set; }

        public string TrailingData { get; set; }
    }

    [TestClass]
    public class StreamTests : TestBase
    {
        [TestMethod]
        public void StreamTest()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("StreamValue"));
            var expected = new StreamClass {Field = stream};
            var actual = Roundtrip(expected);
            Assert.AreEqual(stream.Length, actual.Field.Length);
        }
    }
}
