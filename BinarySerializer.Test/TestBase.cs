using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test
{
    public abstract class TestBase
    {
        private static readonly BinarySerialization.BinarySerializer Serializer = new BinarySerialization.BinarySerializer();

        protected static readonly string[] TestSequence = { "a", "b", "c" };

        private T Roundtrip<T>(T o, out long length)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, o);

            length = stream.Length;

            stream.Position = 0;
            return Serializer.Deserialize<T>(stream);
        }

        protected T Roundtrip<T>(T o) 
        {
            long length;
            return Roundtrip(o, out length);
        }
        
        protected T Roundtrip<T>(T o, long expectedLength) where T : new()
        {
            long length;
            var result = Roundtrip(o, out length);

            Assert.AreEqual(expectedLength, length);

            return result;
        }
    }
}
