using System.IO;
using Xunit;

namespace BinarySerialization.Test.Context
{
        public class ContextTests
    {
        [Fact]
        public void TestContext()
        {
            var contextClass = new ContextClass();
            var serializer = new BinarySerialization.BinarySerializer();

            var context = new Context {SerializeCondtion = false};

            var stream = new MemoryStream();
            serializer.Serialize(stream, contextClass, context);

            context = new Context { SerializeCondtion = true };

            stream = new MemoryStream();
            serializer.Serialize(stream, contextClass, context);

            Assert.Equal(sizeof(int), stream.Length);
        }
    }
}
