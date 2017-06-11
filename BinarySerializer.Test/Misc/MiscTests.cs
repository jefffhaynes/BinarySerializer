using System.IO;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    public class MiscTests : TestBase
    {
        [Fact]
        public void DontFlushTooMuchTest()
        {
            var serializer = new BinarySerializer();
            var expected = new DontFlushTooMuchTestClass();
            var stream = new UnflushableStream();
            
            serializer.Serialize(stream, expected);
        }
    }
}
