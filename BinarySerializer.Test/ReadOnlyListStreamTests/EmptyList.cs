using System;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.ReadOnlyListStreamTests
{
    public class EmptyList : TestBase
    {
        [Fact]
        public void CanSeek()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.True(sut.CanSeek);
            }
        }

        [Fact]
        public void CannotWrite()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.False(sut.CanWrite);
            }
        }

        [Fact]
        public void CannotTimeout()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.False(sut.CanTimeout);
            }
        }

        [Fact]
        public void CanRead()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.True(sut.CanRead);
            }
        }

        [Fact]
        public void IsEmpty()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Equal(0, sut.Length);
            }
        }

        [Fact]
        public void ReadByteReadsEndOfStream()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Equal(-1, sut.ReadByte());
            }
        }

        [Fact]
        public void IsAtStartPosition()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Equal(0, sut.Position);
            }
        }

        [Fact]
        public void DoesNotContainAnyByte()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                var buf = new byte[10];
                var bytesRead = sut.Read(buf, 0, buf.Length);
                Assert.Equal(0, bytesRead);
            }
        }

        [Fact]
        public void CannotWriteAnyBytesIntoStream()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                var buf = new byte[] {0x00, 0x01, 0x02};
                Assert.Throws<ReadOnlyStreamException>(() => sut.Write(buf, 0, buf.Length));
            }
        }

        [Fact]
        public void CanSeekOneByteForward()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                var newPosition = sut.Seek(1, SeekOrigin.Begin);
                Assert.Equal(1, newPosition);
            }
        }

        [Fact]
        public void CanSetPositionToOne()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                sut.Position = 1;
            }
        }

        [Fact]
        public void SetLengthThrows()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Throws<NotSupportedException>(() => sut.SetLength(0));
            }
        }

        [Fact]
        public void ThrowsWhenSettingPositionToMinusOne()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Throws<SeekOutOfRangeException>(() => sut.Position = -1);
            }
        }

        [Fact]
        public void CanSetPositionToZero()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                sut.Position = 0;
            }
        }

        [Fact]
        public void ThrowsWhenSeekBackwardFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Throws<SeekOutOfRangeException>(() => sut.Seek(-1, SeekOrigin.Begin));
            }
        }

        [Fact]
        public void ReadsEndOfStream()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                Assert.Equal(-1, sut.ReadByte());
            }
        }

        [Fact]
        public void CanSeekZeroBytesFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                var newPosition = sut.Seek(0, SeekOrigin.Begin);
                Assert.Equal(0, newPosition);
            }
        }

        [Fact]
        public void CanSeekZeroBytesFromEnd()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                var newPosition = sut.Seek(0, SeekOrigin.End);
                Assert.Equal(0, newPosition);
            }
        }

        [Fact]
        public void CanSeekZeroBytesFromCurrentPosition()
        {
            using (var sut = new ReadOnlyListStream(_emptyList))
            {
                var newPosition = sut.Seek(0, SeekOrigin.Current);
                Assert.Equal(0, newPosition);
            }
        }
    }
}