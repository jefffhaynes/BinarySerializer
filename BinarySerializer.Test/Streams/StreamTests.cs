using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Streams
{
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
