using System.IO;
using Xunit;

namespace BinarySerialization.Test.Misc
{
        public class IOExceptionTest
    {
        [Fact]
        public void ShouldThrowIOExceptionNotInvalidOperationExceptionTest()
        {
            var stream = new UnreadableStream();
            var serializer = new BinarySerialization.BinarySerializer();
            var e = Record.Exception( () => serializer.Deserialize<int>(stream));
            Assert.NotNull(e);
            Assert.IsType<IOException>(e);
        }
    }
}