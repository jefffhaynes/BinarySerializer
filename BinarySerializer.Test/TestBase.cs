using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test
{
    public abstract class TestBase
    {
        private static readonly BinarySerialization.BinarySerializer Serializer = new BinarySerialization.BinarySerializer();

        protected static readonly string[] TestSequence = { "a", "b", "c" };

        static TestBase()
        {
            Serializer.MemberDeserialized += (sender, args) => Console.WriteLine(args.MemberName + ": " + args.Value);
        }

        private T Roundtrip<T>(T o, out byte[] data)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, o);

            stream.Position = 0;
            data = stream.ToArray();
            return Serializer.Deserialize<T>(stream);
        }

        protected T Roundtrip<T>(T o) 
        {
            byte[] data;
            return Roundtrip(o, out data);
        }
        
        protected T Roundtrip<T>(T o, long expectedLength)
        {
            byte[] data;
            var result = Roundtrip(o, out data);

            Assert.AreEqual(expectedLength, data.Length);

            return result;
        }

        protected T Roundtrip<T>(T o, byte[] expectedValue)
        {
            byte[] data;
            var result = Roundtrip(o, out data);

            for (int i = 0; i < expectedValue.Length; i++)
            {
                var expected = expectedValue[i];
                var actual = data[i];

                Assert.AreEqual(expected, actual,
                    string.Format("Value at position {0} does not match expected value.", i));
            }

            return result;
        }
    }
}
