using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test
{
    public abstract class TestBase
    {
        protected static readonly BinarySerializer Serializer = new BinarySerializer();

        protected static readonly string[] TestSequence = { "a", "b", "c" };
        protected static readonly int[] PrimitiveTestSequence = { 1, 2, 3 };

        static TestBase()
        {
            Serializer.MemberDeserialized += (sender, args) =>
            {
                var bytes = args.Value as byte[];
                if (bytes != null)
                    Console.WriteLine(args.MemberName + ": " + BitConverter.ToString(bytes));
                else
                    Console.WriteLine(args.MemberName + ": " + args.Value);
            };
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
                    $"Value at position {i} does not match expected value.");
            }

            return result;
        }

        protected T Deserialize<T>(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        protected T Deserialize<T>(byte[] data)
        {
            return Serializer.Deserialize<T>(data);
        }
    }
}
