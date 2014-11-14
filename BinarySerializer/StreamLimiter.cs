using System;
using System.IO;

namespace BinarySerialization
{
    internal class StreamLimiter : Stream
    {
        private long _length;
        private long _position;

        private StreamLimiter(Stream source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        public StreamLimiter(Stream source, long length) : this(source)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "The length cannot be negative");

            _length = length;
        }

        /// <summary>
        ///     The underlying source <see cref="Stream" />.
        /// </summary>
        public Stream Source { get; private set; }

        public override bool CanRead
        {
            get { return Source.CanRead; }
        }

        public override bool CanSeek
        {
            get { return Source.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _position; }

            set
            {
                var delta = value - _position;
                Source.Position += delta;
                _position = value;
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
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
            if (count > Length - Position)
            {
                count = Math.Max(0, (int) (Length - Position));
            }

            if (count == 0)
                return 0;

            int read = Source.Read(buffer, offset, count);
            _position += read;

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}