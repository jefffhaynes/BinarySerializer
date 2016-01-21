using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerialization
{
    internal class LimitedStream : Stream
    {
        private readonly bool _canSeek;
        private readonly long _length;
        private readonly bool _unbounded;

        public LimitedStream(Stream source) : this(source, long.MaxValue)
        {
            _unbounded = true;
        }

        public LimitedStream(Stream source, long maxLength)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), "Cannot be negative.");

            Source = source;
            MaxLength = maxLength;

            /* Store for performance */
            _canSeek = source.CanSeek;

            if(_canSeek)
                _length = source.Length;
        }

        public long RelativePosition { get; set; }

        public long GlobalRelativePosition
        {
            get
            {
                return Ancestors.Sum(limiter => limiter.RelativePosition);
            }
        }

        private IEnumerable<LimitedStream> Ancestors
        {
            get
            {
                var parent = this;

                while (parent != null)
                {
                    yield return parent;
                    parent = parent.Source as LimitedStream;
                }
            }
        }

        /// <summary>
        ///     The underlying source <see cref="Stream" />.
        /// </summary>
        public Stream Source { get; }

        public bool IsAtLimit
        {
            get
            {
                if (_unbounded)
                    return false;

                return Position >= MaxLength;
            }
        }

        public override bool CanRead => Source.CanRead;

        public override bool CanSeek => _canSeek;

        public override bool CanWrite => Source.CanWrite;

        public long MaxLength { get; }

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

        public long AvailableForWriting => MaxLength - Position;

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