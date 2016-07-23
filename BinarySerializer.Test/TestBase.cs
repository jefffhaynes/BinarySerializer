using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test
{
    public abstract class TestBase
    {
        protected static readonly BinarySerializer Serializer = new BinarySerializer();

        protected static readonly string[] TestSequence = {"a", "b", "c"};
        protected static readonly int[] PrimitiveTestSequence = {1, 2, 3};

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

        public T Roundtrip<T>(T o)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, o);

            stream.Position = 0;
            return Serializer.Deserialize<T>(stream);
        }

        protected T Roundtrip<T>(T o, long expectedLength)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, o);

            stream.Position = 0;
            var data = stream.ToArray();

            Assert.AreEqual(expectedLength, data.Length);

            return Serializer.Deserialize<T>(stream);
        }

        protected T Roundtrip<T>(T o, byte[] expectedValue)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, o);

            stream.Position = 0;
            var data = stream.ToArray();

            AssertEqual(expectedValue, data);
            
            return Serializer.Deserialize<T>(stream);
        }

        private void AssertEqual(byte[] expected, byte[] actual)
        {
            var length = Math.Min(expected.Length, actual.Length);

            for (var i = 0; i < length; i++)
            {
                var e = expected[i];
                var a = actual[i];

                Assert.AreEqual(e, a, $"Value at position {i} does not match expected value.");
            }

            Assert.AreEqual(expected.Length, actual.Length, "Sequence lengths do not match");
        }

        protected T RoundtripReverse<T>(byte[] data)
        {
            var o = Deserialize<T>(data);

            var stream = new MemoryStream();
            Serializer.Serialize(stream, o);

            AssertEqual(data, stream.ToArray());

            return o;
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