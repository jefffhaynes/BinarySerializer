using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.ReadOnlyListStreamTests
{
    public class ListWithContent : TestBase
    {
        [Fact]
        public void CanSeek()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.True(sut.CanSeek);
            }
        }

        [Fact]
        public void CannotWrite()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.False(sut.CanWrite);
            }
        }

        [Fact]
        public void CannotTimeout()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.False(sut.CanTimeout);
            }
        }

        [Fact]
        public void CanRead()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.True(sut.CanRead);
            }
        }

        [Fact]
        public void StreamLengthMatchesListCount()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.Equal(_list.Count, sut.Length);
            }
        }

        [Fact]
        public void IsAtStartPositionAfterCreation()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.Equal(0, sut.Position);
            }
        }

        [Fact]
        public void CanReadAllBytes()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[256];
                var bytesRead = sut.Read(buf, 0, buf.Length);

                Assert.Equal(_list.Count, bytesRead);
                Assert.Equal(_list, buf.Take(bytesRead));
            }
        }

        [Fact]
        public void CannotWriteAnyBytesIntoStream()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[] {0x00, 0x01, 0x02};
                Assert.Throws<ReadOnlyStreamException>(() => sut.Write(buf, 0, buf.Length));
            }
        }

        [Fact]
        public void CanSeekBeyondListBondariesFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(_list.Count, SeekOrigin.Begin);
                Assert.Equal(_list.Count, newPosition);
            }
        }

        [Fact]
        public void ReadsEndOfStreamAfterSeekingToListEnd()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(_list.Count, SeekOrigin.Begin);
                Assert.Equal(-1, sut.ReadByte());
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
        public void ReadFirstByteAfterCreation()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.Equal(0, sut.ReadByte());
            }
        }

        [Fact]
        public void ReadsAllBytesInCorrectOrder()
        {
            var result = new List<int>();
            using (var sut = new ReadOnlyListStream(_list))
            {
                for (var position = 0; position < _list.Count; position++)
                {
                    result.Add(sut.ReadByte());
                }
            }
            Assert.Equal(_list.Select(value => (int) value), result);
        }

        [Fact]
        public void WritesBytesAtCorrectPositionWhenReadingWithOffset()
        {
            var buf = new byte[50];
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Read(buf, 5, 5);
            }
            Assert.Equal(
                new byte[] {
                    0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x01, 0x02, 0x03, 0x04
                },
                buf.Take(10));
        }

        [Fact]
        public void ReadsOnlyFiveBytesAfterSeekingFiveBytesForward()
        {
            var buf = new byte[50];
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(5, SeekOrigin.Begin);
                sut.Read(buf, 5, 10);
            }
            Assert.Equal(
                new byte[] {
                    0x00, 0x00, 0x00, 0x00, 0x00,
                    0x05, 0x06, 0x07, 0x08, 0x09,
                    0x00, 0x00, 0x00, 0x00, 0x00
                },
                buf.Take(15));
        }

        [Fact]
        public void ReadsZeroBytesAfterSeekingTenBytesForward()
        {
            var buf = new byte[50];
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(10, SeekOrigin.Begin);
                Assert.Equal(0, sut.Read(buf, 5, 10));
            }
        }

        [Fact]
        public void ReadsExpectedValueAfterSeekingOneByteForwardFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(1, SeekOrigin.Begin);
                Assert.Equal(1, sut.ReadByte());
            }
        }

        [Fact]
        public void ReadsExpectedValueAfterSeekingOneByteForwardFromCurrent()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(1, SeekOrigin.Current);
                Assert.Equal(1, sut.ReadByte());
            }
        }

        [Fact]
        public void ReadsExpectedValueAfterSeekingOneByteBackwardFromEnd()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(-1, SeekOrigin.End);
                Assert.Equal(9, sut.ReadByte());
            }
        }

        [Fact]
        public void CanSetPositionToOne()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Position = 1;
            }
        }

        [Fact]
        public void ThrowsWhenSettingPositionToMinusOne()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.Throws<SeekOutOfRangeException>(() => sut.Position = -1);
            }
        }

        [Fact]
        public void CanSetPositionToZero()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Position = 0;
            }
        }

        [Fact]
        public void HasCorrectPositionAfterSeekingFiveBytesForwardFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                sut.Seek(5, SeekOrigin.Begin);
                Assert.Equal(5, sut.Position);
            }
        }

        [Fact]
        public void HasCorrectPositionAfterReadingFiveBytesFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[10];
                sut.Read(buf, 0, 5);
                Assert.Equal(5, sut.Position);
            }
        }

        [Fact]
        public void CanSeekOneByteForwardFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(1, SeekOrigin.Begin);
                Assert.Equal(1, newPosition);
            }
        }

        [Fact]
        public void CanSeekOneByteBackwardFromEnd()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(-1, SeekOrigin.End);
                Assert.Equal(9, newPosition);
            }
        }

        [Fact]
        public void CanSeekToListEnd()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(0, SeekOrigin.End);
                Assert.Equal(10, newPosition);
            }
        }

        [Fact]
        public void ThrowsWhenSeekBackwardFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.Throws<SeekOutOfRangeException>(() => sut.Seek(-1, SeekOrigin.Begin));
            }
        }

        [Fact]
        public void ThrowsWhenSeekBackwardFromEnd()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                Assert.Throws<SeekOutOfRangeException>(() => sut.Seek(-100, SeekOrigin.End));
            }
        }

        [Fact]
        public void CanSeekZeroBytesFromBegin()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(0, SeekOrigin.Begin);
                Assert.Equal(0, newPosition);
            }
        }

        [Fact]
        public void CanSeekZeroBytesFromEnd()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(0, SeekOrigin.End);
                Assert.Equal(_list.Count, newPosition);
            }
        }

        [Fact]
        public void CanSeekZeroBytesFromCurrentPosition()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var newPosition = sut.Seek(0, SeekOrigin.Current);
                Assert.Equal(0, newPosition);
            }
        }

        [Fact]
        public void ReadsOneByteAfterSeekingToTheLastItem()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[10];
                sut.Seek(9, SeekOrigin.Begin);
                Assert.Equal(1, sut.Read(buf, 0, 10));
            }
        }

        [Fact]
        public void ReadsFiveBytesAfterSeekingToTheSixthItem()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[10];
                sut.Seek(5, SeekOrigin.Begin);
                Assert.Equal(5, sut.Read(buf, 0, 10));
            }
        }

        [Fact]
        public void ReadsFiveBytesInCorrectOrderAfterSeekingToTheSixthItem()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[10];
                sut.Seek(5, SeekOrigin.Begin);
                sut.Read(buf, 0, 10);
                Assert.Equal(
                    new byte[] {
                        0x05, 0x06, 0x07, 0x08, 0x09,
                        0x00, 0x00, 0x00, 0x00, 0x00
                    },
                    buf);
            }
        }

        [Fact]
        public void ReadsFiveBytesInCorrectOrderAfterSettingPositionToTheSixthItem()
        {
            using (var sut = new ReadOnlyListStream(_list))
            {
                var buf = new byte[10];
                sut.Position = 5;
                sut.Read(buf, 0, 10);
                Assert.Equal(
                    new byte[] {
                        0x05, 0x06, 0x07, 0x08, 0x09,
                        0x00, 0x00, 0x00, 0x00, 0x00
                    },
                    buf);
            }
        }
    }
}