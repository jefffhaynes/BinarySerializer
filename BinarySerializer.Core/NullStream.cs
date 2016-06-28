using System;
using System.IO;

namespace BinarySerialization
{
    internal class NullStream : Stream
    {
        private long _length;
        
        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }

            return Position;
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if(offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            Position += offset + count;

            _length = Math.Max(_length, Position);
        }

        public override bool CanRead => false;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position { get; set; }
    }
}
