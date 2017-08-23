using System;
using System.Collections.Generic;
using System.IO;

namespace BinarySerialization
{
    /// <summary>
    ///    Wraps read only list in stream
    /// </summary>
    internal sealed class ReadOnlyListStream : Stream
    {
        private const int EndOfStream = -1;
        private readonly IReadOnlyList<byte> _list;
        private int _position;

        /// <summary>
        ///     Creates a stream instance using a read only list as backend.
        /// </summary>
        /// <param name="list">A list of bytes the stream should operate on.</param>
        public ReadOnlyListStream(IReadOnlyList<byte> list)
        {
            _list = list ?? throw new ArgumentNullException(nameof(list));
        }

        public override bool CanTimeout => false;
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _list.Count;

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            // Ignore, stream is read only
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            if (_position >= _list.Count) return 0;

            var bytesCopied = Math.Max(
                Math.Min(_list.Count - _position, count),
                0);

            for (var i = 0; i < bytesCopied; i++)
                buffer[offset + i] = _list[_position++];
            return bytesCopied;
        }

        public override int ReadByte()
        {
            return _position >= _list.Count
                ? EndOfStream
                : _list[_position++];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var startPosition = GetStartPosition(origin);

            var newPosition = startPosition + (int) offset;
            if (newPosition < 0)
                ThrowSeekOutOfRangeException(offset, origin);

            return _position = newPosition;
        }

        private int GetStartPosition(SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return 0;
                case SeekOrigin.Current:
                    return _position;
                case SeekOrigin.End:
                    return _list.Count;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }
        }

        private void ThrowSeekOutOfRangeException(long offset, SeekOrigin origin)
        {
            var message = $"Invalid seek offset: {offset}, origin: {origin}, current position: {_position}";
            throw new SeekOutOfRangeException(message);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new ReadOnlyStreamException();
        }
    }
}