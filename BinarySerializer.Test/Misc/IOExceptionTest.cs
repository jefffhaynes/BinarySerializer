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
            var serializer = new BinarySerializer();
            Assert.Throws<IOException>(() => serializer.Deserialize<int>(stream));
        }
    }
}