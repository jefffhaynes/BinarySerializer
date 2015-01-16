using System;
using System.IO;

namespace BinarySerialization
{
    internal class StreamLimiter : Stream
    {
        private readonly bool _canSeek;
        private readonly long _length;
        private readonly long _maxLength;

        private long _position;

        public StreamLimiter(Stream source, long maxLength = long.MaxValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength", "Cannot be negative.");

            Source = source;
            _maxLength = maxLength;

            /* Store for performance */
            _canSeek = source.CanSeek;

            if(_canSeek)
                _length = source.Length;
        }

        /// <summary>
        ///     The underlying source <see cref="Stream" />.
        /// </summary>
        public Stream Source { get; private set; }

        public bool IsAtLimit
        {
            get { return Position >= MaxLength; }
        }

        public override bool CanRead
        {
            get { return Source.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _canSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public long MaxLength
        {
            get { return _maxLength; }
        }

        public override long Length
        {
            get
            {
                if (!_canSeek)
                    return Source.Length;

                return _length;
            }
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
            Source.Flush();
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
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > MaxLength - Position)
            {
                count = Math.Max(0, (int) (MaxLength - Position));
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