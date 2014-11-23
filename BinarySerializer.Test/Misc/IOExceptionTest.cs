using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Misc
{
    [TestClass]
    public class IOExceptionTest
    {
        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void ShouldThrowIOExceptionNotInvalidOperationExceptionTest()
        {
            var stream = new UnreadableStream();
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Deserialize<int>(stream);
        }
    }
}