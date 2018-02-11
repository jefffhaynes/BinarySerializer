using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    /// <summary>
    ///     Provides a bounded stream.
    /// </summary>
    public class BoundedStream : Stream
    {
        private const int BitsPerByte = 8;
        private readonly bool _canSeek;
        private readonly long _length;
        private readonly Func<FieldLength> _maxLengthDelegate;
        private readonly string _name;
        private readonly BoundedStream _root;

        private byte _bitBuffer;

        private int _bitOffset;

        internal BoundedStream(Stream source, string name, Func<FieldLength> maxLengthDelegate = null)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            _name = name;
            _maxLengthDelegate = maxLengthDelegate;

            /* Store for performance */
            _canSeek = source.CanSeek;

            if (_canSeek)
            {
                _length = source.Length;
            }

            _root = this;

            while (_root.Source is BoundedStream)
            {
                _root = (BoundedStream) _root.Source;
            }
        }

        /// <summary>
        ///     Gets the current offset in the serialized graph.
        /// </summary>
        public FieldLength GlobalPosition => _root.RelativePosition;

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
        public FieldLength MaxLength => _maxLengthDelegate?.Invoke();

        /// <summary>
        ///     Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => !_canSeek ? Source.Length : _length;

        /// <summary>
        ///     Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get => (long) RelativePosition.ByteCount;

            set
            {
                var delta = value - RelativePosition;
                Source.Position += (long) delta.ByteCount;
                RelativePosition = value;
            }
        }

        internal FieldLength RelativePosition { get; set; } = FieldLength.Zero;

        internal bool IsAtLimit
        {
            get
            {
                if (MaxLength != null)
                {
                    return Position >= MaxLength;
                }

                return Source is BoundedStream source && source.IsAtLimit;
            }
        }

        internal FieldLength AvailableForReading
        {
            get
            {
                FieldLength maxLength;

                if (MaxLength == null)
                {
                    if (Source is BoundedStream source)
                    {
                        return source.AvailableForReading;
                    }

                    maxLength = FieldLength.MaxValue;
                }
                else
                {
                    maxLength = MaxLength;
                }

                if (!_canSeek)
                {
                    return maxLength - Position;
                }

                return FieldLength.Min(maxLength, Length) - Position;
            }
        }

        internal FieldLength AvailableForWriting
        {
            get
            {
                if (MaxLength != null)
                {
                    return MaxLength - Position;
                }

                var source = Source as BoundedStream;
                return source?.AvailableForWriting ?? long.MaxValue;
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
            count = ClampCount(count);

            if (count == 0)
            {
                return 0;
            }

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
            if (offset != 0)
            {
                throw new ArgumentException(nameof(offset));
            }

            WriteImpl(buffer, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (offset != 0)
            {
                throw new ArgumentException(nameof(offset));
            }

            return WriteAsyncImpl(buffer, count, cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, FieldLength length, CancellationToken cancellationToken)
        {
            return WriteAsyncImpl(buffer, length, cancellationToken);
        }
        
        private async Task WriteAsyncImpl(byte[] buffer, FieldLength length, CancellationToken cancellationToken)
        {
            if (length == FieldLength.Zero)
            {
                return;
            }

            WriteCheck(length);

            if (Source is BoundedStream boundedStream && !(this is TapStream))
            {
                await boundedStream.WriteAsync(buffer, length, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (length.BitCount == 0 && _bitOffset == 0)
                {
                    // trivial byte-aligned case, write to underlying stream
                    await WriteByteAlignedAsync(buffer, (int) length.ByteCount, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    // collect bits in this, the bottom bounded stream
                    for (ulong i = 0; i < length.ByteCount; i++)
                    {
                        await WriteBitsAsync(buffer[i], BitsPerByte, cancellationToken).ConfigureAwait(false);
                    }

                    await WriteBitsAsync(buffer[length.ByteCount], length.BitCount, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            RelativePosition += length;
        }

        protected virtual void WriteByteAligned(byte[] buffer, int length)
        {
            Source.Write(buffer, 0, length);
        }

        protected virtual Task WriteByteAlignedAsync(byte[] buffer, int length, CancellationToken cancellationToken)
        {
            return Source.WriteAsync(buffer, 0, length, cancellationToken);
        }
        
        public void Write(byte[] buffer, FieldLength length)
        {
            WriteImpl(buffer, length);
        }
        
        private void WriteImpl(byte[] buffer, FieldLength length)
        {
            if (length == FieldLength.Zero)
            {
                return;
            }

            WriteCheck(length);

            if (Source is BoundedStream boundedStream && !(this is TapStream))
            {
                boundedStream.Write(buffer, length);
            }
            else
            {
                if (length.BitCount == 0 && _bitOffset == 0)
                {
                    // trivial byte-aligned case
                    WriteByteAligned(buffer, (int) length.ByteCount);
                }
                else
                {
                    for (ulong i = 0; i < length.ByteCount; i++)
                    {
                        WriteBits(buffer[i], BitsPerByte);
                    }

                    WriteBits(buffer[length.ByteCount], length.BitCount);
                }
            }

            RelativePosition += length;
        }
        
        private void WriteCheck(FieldLength length)
        {
            if (MaxLength != null && length > MaxLength - Position)
            {
                throw new InvalidOperationException("Unable to write beyond end of stream limit.");
            }
        }

        private void WriteBits(byte value, int count)
        {
            if (count == 0)
            {
                return;
            }

            var remaining = BitsPerByte - _bitOffset;
            var shiftedValue = value << _bitOffset;
            _bitBuffer |= (byte) shiftedValue;
            _bitOffset += count;

            if (_bitOffset >= BitsPerByte)
            {
                var data = new[] { _bitBuffer };
                WriteByteAligned(data, data.Length);
                _bitBuffer = (byte) (value >> remaining);
            }

            _bitOffset %= BitsPerByte;
        }

        private async Task WriteBitsAsync(byte value, int count, CancellationToken cancellationToken)
        {
            if (count == 0)
            {
                return;
            }

            var remaining = BitsPerByte - _bitOffset;
            var shiftedValue = value << _bitOffset;
            _bitBuffer |= (byte) shiftedValue;
            _bitOffset += count;

            if (_bitOffset >= BitsPerByte)
            {
                var data = new[] {_bitBuffer};
                await WriteByteAlignedAsync(data, data.Length, cancellationToken).ConfigureAwait(false);
                _bitBuffer = (byte) (value >> remaining);
            }

            _bitOffset %= BitsPerByte;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            count = ClampCount(count);

            if (count == 0)
            {
                return 0;
            }

            var read = await Source.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            RelativePosition += read;

            return read;
        }

        private int ClampCount(int count)
        {
            if (MaxLength != null && count > MaxLength - Position)
            {
                count = Math.Max(0, (int) ((long) MaxLength.ByteCount - Position));
            }

            return count;
        }
    }
}