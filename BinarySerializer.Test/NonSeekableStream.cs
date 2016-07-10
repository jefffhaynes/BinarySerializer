using System;
using System.IO;

namespace BinarySerialization.Test
{
    /// <summary>
    ///     Makes underlying stream appear as non-seekable.
    /// </summary>
    internal class NonSeekableStream : Stream
    {
        private readonly Stream _stream;

        public NonSeekableStream()
        {
        }

        public NonSeekableStream(Stream stream)
        {
            _stream = stream;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get
            {
                if (_stream == null)
                    return true;

                return _stream.CanWrite;
            }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_stream == null)
                return;

            _stream.Write(buffer, offset, count);
        }
    }
}