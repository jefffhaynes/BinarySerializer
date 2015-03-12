using System;
using System.IO;

namespace BinarySerialization
{
    internal class StreamLimiter : Stream
    {
        private readonly bool _canSeek;
        private readonly long _length;
        private readonly long _maxLength;

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

        public long RelativePosition { get; set; }

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
            get { return Source.CanWrite; }
        }

        public long MaxLength
        {
            get { return _maxLength; }
        }

        public override long Length
        {
            get
            {
                /* If we can't seek, might as well go to the source */
                if (!_canSeek)
                    return Source.Length;

                return _length;
            }
        }

        public long AvailableForReading
        {
            get
            {
                if (!_canSeek)
                    return MaxLength - Position;

                return Math.Min(MaxLength, Length) - Position;
            }
        }

        public long AvailableForWriting
        {
            get
            {
                return MaxLength - Position;
            }
        }

        public override long Position
        {
            get { return RelativePosition; }

            set
            {
                var delta = value - RelativePosition;
                Source.Position += delta;
                RelativePosition = value;
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
            RelativePosition += read;

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;

            if (count > MaxLength - Position)
            {
                throw new InvalidOperationException("Unable to write beyond end of stream limit.");
            }

            Source.Write(buffer, offset, count);
            RelativePosition += count;
        }
    }
}