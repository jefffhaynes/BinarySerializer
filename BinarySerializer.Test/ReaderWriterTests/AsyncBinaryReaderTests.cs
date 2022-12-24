using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.ReaderWriterTests
{
    [TestClass]
    public class AsyncBinaryReaderTests : TestBase
    {
        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public async Task ReadCharAsyncASCIITest()
        {
            var encoding = System.Text.Encoding.ASCII;

            var expected = 'a';
            var data = encoding.GetBytes(expected.ToString());
            var stream = new MemoryStream(data);
            var boundedStream = new BoundedStream(stream, string.Empty);
            var reader = new AsyncBinaryReader(boundedStream, encoding);
            var actual = await reader.ReadCharAsync(CancellationToken.None);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public async Task ReadCharAsyncUTF8Test()
        {
            var encoding = System.Text.Encoding.UTF8;

            var expected = 'ش';
            var data = encoding.GetBytes(expected.ToString());
            var stream = new MemoryStream(data);
            var boundedStream = new BoundedStream(stream, string.Empty);
            var reader = new AsyncBinaryReader(boundedStream, encoding);
            var actual = await reader.ReadCharAsync(CancellationToken.None);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public async Task ReadCharAsyncUTF16Test()
        {
            var encoding = System.Text.Encoding.Unicode;

            var expected = 'ش';
            var data = encoding.GetBytes(expected.ToString());
            var stream = new MemoryStream(data);
            var boundedStream = new BoundedStream(stream, string.Empty);
            var reader = new AsyncBinaryReader(boundedStream, encoding);
            var actual = await reader.ReadCharAsync(CancellationToken.None);
            Assert.AreEqual(expected, actual);
        }
    }
}
