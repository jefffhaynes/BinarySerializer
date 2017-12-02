using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue78
{
    public class Issue78Tests
    {
        [Fact]
        public void CrcThreadSafteyTest()
        {
            var serializer = new BinarySerializer();
            var frame = new Frame { Payload = new Payload { Number = 42, String = "Hello World" } };
            serializer.SerializeAsync(new MemoryStream(), new CrcWrap { Frame = frame }).ConfigureAwait(false).GetAwaiter()
                .GetResult();

            List<ushort> crcs = new List<ushort>();

            for (int i = 0; i < 50; i++)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var frameStream = new MemoryStream();
#pragma warning disable 618
                    serializer.Serialize(frameStream, new CrcWrap { Frame = frame });
#pragma warning restore 618
                    var frameBytes = frameStream.ToArray();

                    Crc16 crc16 = new Crc16(0x1021, 0xffff);

                    crc16.Compute(frameBytes, 0, frameBytes.Length);

                    var crc = crc16.ComputeFinal();

                    crcs.Add(crc);
                });
            }

            Assert.All(crcs, crc => Assert.Equal(0, crc));
        }
    }
}
