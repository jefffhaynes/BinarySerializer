using System;
using System.IO;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test
{
        public class StreamletTest
    {
        private static readonly byte[] SourceData = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

        private void AssertRead(int offset, int length)
        {
            var source = new MemoryStream(SourceData);
            var streamlet = new Streamlet(source, offset, length);

            var block = new byte[SourceData.Length];
            var read = streamlet.Read(block, 0, block.Length);

            var sourceBalance = source.Length - offset;
            var expectedRead = Math.Min(sourceBalance, length);

            Assert.Equal(expectedRead, read);

            var expectedSequence = SourceData.Skip(offset).Take(read);
            var sequence = block.Take(read);
            Assert.True(expectedSequence.SequenceEqual(sequence));

            Assert.Equal(offset + read, source.Position);
        }

        private void AssertSeek(int offset, int length, int seekOffset, SeekOrigin seekOrigin)
        {
            var source = new MemoryStream(SourceData);
            var streamlet = new Streamlet(source, offset, length);

            streamlet.Seek(seekOffset, seekOrigin);

            long sourcePosition;
            long position;

            switch (seekOrigin)
            {
                case SeekOrigin.Begin:
                case SeekOrigin.Current:
                    sourcePosition = offset + seekOffset;
                    position = seekOffset;
                    break;
                case SeekOrigin.End:
                    sourcePosition = offset + length - seekOffset;
                    position = length - seekOffset;
                    break;
                default:
                    throw new NotSupportedException();
            }

            Assert.Equal(sourcePosition, source.Position);
            Assert.Equal(position, streamlet.Position);
        }

        [Fact]
        public void ReadFullTest()
        {
            AssertRead(0, SourceData.Length);
        }

        [Fact]
        public void ReadPartialTest()
        {
            AssertRead(0, 4);
        }

        [Fact]
        public void ReadPartialOffsetTest()
        {
            AssertRead(2, 4);
        }
    }
}
