using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue38
{
    
    public class Issue38Tests : TestBase
    {
        //[Fact]
        public void ConstructOnceTest()
        {
            var expected = new ConstructOnceClass();
            Roundtrip(expected);

            Assert.Equal(1, ConstructOnceClass.Count);
        }

        //[Fact]
        public void DeserializeMessageTest()
        {
            var serializer = new BinarySerializer {Endianness = BinarySerialization.Endianness.Little};
            serializer.MemberDeserializing +=
                (sender, args) => { Debug.Write($"Deserializing {args.MemberName}, stream offset {args.Offset}"); };

            serializer.MemberDeserialized +=
                (sender, args) =>
                {
                    Debug.Write($"Deserialized {args.MemberName}, ({args.Value ?? "null"}), stream offset {args.Offset}");
                };
            var inBytes = new byte[]
            {
                0x02, 0x00, 0x78, 0xFF, 0x00, 0x00, 0x30, 0x60, 0x03, 0x01
            };

            using (var stream = new MemoryStream(inBytes))
            {
                var actualObj = serializer.Deserialize<MachineState1>(stream);
                Assert.Null(actualObj);
            }
        }
    }
}