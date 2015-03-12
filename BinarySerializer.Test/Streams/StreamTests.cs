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
    }
}
