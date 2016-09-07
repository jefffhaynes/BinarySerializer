using System;
using System.IO;

namespace BinarySerialization
{
    /// <summary>
    ///     Provides a bounded stream.
    /// </summary>
    public class BoundedStream : Stream
    {
        private readonly BoundedStream _root;
        private readonly bool _canSeek;
        private readonly long _length;
        private readonly Func<long?> _maxLengthDelegate; 
        
        internal BoundedStream(Stream source, Func<long?> maxLengthDelegate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            Source = source;
            _maxLengthDelegate = maxLengthDelegate;

            /* Store for performance */
            _canSeek = source.CanSeek;

            if (_canSeek)
                _length = source.Length;

            _root = this;

            while (_root.Source is BoundedStream)
            {
                _root = (BoundedStream) _root.Source;
            }
        }

        /// <summary>
        ///     Gets the current offset in the serialized graph.
        /// </summary>
        public long GlobalPosition => _root.RelativePosition;

        /// <summary>
        ///     The underlying source <see cref="Stream" />.
        /// </summary>
        public Stream Source { get; }

        /// <summary>
        ///     Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => Source.CanRead;

        /// <summary>
        ///     Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => _canSeek;

        /// <summary>
        ///     Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => Source.CanWrite;

        /// <summary>
        ///     Gets the maximum length of the stream in bytes if bounded.  Returns null if stream is unbounded.
        /// </summary>
        public long? MaxLength => _maxLengthDelegate?.Invoke();

        /// <summary>
        ///     Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => !_canSeek ? Source.Length : _length;

        /// <summary>
        ///     Gets or sets the position within the current stream.
        /// </summary>
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

        /// <summary>
        ///     Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            Source.Flush();
        }

        /// <summary>
        ///     Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A <see cref="SeekOrigin" /> object indicating the reference point used to obtain the new position.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Gets the length in bytes of the stream.
        /// </summary>
        /// <param name="value">A long value representing the length of the stream in bytes.</param>
        /// <exception cref="NotSupportedException">
        ///     This method exists only to support inheritance from <see cref="Stream" />, and
        ///     cannot be used.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Reads a sequence of bytes from the current stream and advances the position within the stream by the number of
        ///     bytes read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
        ///     values between offset and (offset + count - 1) replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data read from the current
        ///     stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        ///     many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (MaxLength != null && count > MaxLength - Position)
            {
                count = Math.Max(0, (int) (MaxLength - Position));
            }

            if (count == 0)
                return 0;

            var read = Source.Read(buffer, offset, count);
            RelativePosition += read;

            return read;
        }

        /// <summary>
        ///     Writes a sequence of bytes to the current stream and advances the current position within this stream by the number
        ///     of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="InvalidOperationException">count is greater than the stream length.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;

            if (MaxLength != null && count > MaxLength - Position)
            {
                throw new InvalidOperationException("Unable to write beyond end of stream limit.");
            }

            Source.Write(buffer, offset, count);
            RelativePosition += count;
        }
        
        internal long RelativePosition { get; set; }
        
        internal bool IsAtLimit
        {
            get
            {
                if (MaxLength == null)
                    return false;

                return Position >= MaxLength;
            }
        }
        
        internal long AvailableForReading
        {
            get
            {
                var maxLength = MaxLength ?? long.MaxValue;

                if (!_canSeek)
                    return maxLength - Position;

                return Math.Min(maxLength, Length) - Position;
            }
        }

        internal long AvailableForWriting
        {
            get
            {
                if (MaxLength == null)
                    return long.MaxValue;

                return MaxLength.Value - Position;
            }
        }
    }
}