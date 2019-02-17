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

            while (_root.Source is BoundedStream root)
            {
                _root = root;
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
            get => RelativePosition.ByteCount;

            set
            {
                var delta = value - RelativePosition;
                Source.Position += delta.ByteCount;
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

        protected virtual bool IsByteBarrier => false;

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
            return (int) ReadImpl(buffer, count).ByteCount;
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

        public void Write(byte[] buffer, FieldLength length)
        {
            WriteImpl(buffer, length);
        }

        public Task WriteAsync(byte[] buffer, FieldLength length, CancellationToken cancellationToken)
        {
            return WriteAsyncImpl(buffer, length, cancellationToken);
        }

        private void WriteImpl(byte[] buffer, FieldLength length)
        {
            if (length == FieldLength.Zero)
            {
                return;
            }

            WriteCheck(length);

            if (Source is BoundedStream boundedStream && !IsByteBarrier)
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
                    var lastByteIndex = length.BitCount == 0 ? length.ByteCount - 1 : length.ByteCount;
                    var bitCount = length.BitCount == 0 ? BitsPerByte : length.BitCount;

                    WriteBits(buffer[lastByteIndex], bitCount);

                    for (long i = 0; i < lastByteIndex; i++)
                    {
                        WriteBits(buffer[lastByteIndex - (i + 1)], BitsPerByte);
                    }
                }
            }

            RelativePosition += length;
        }

        private async Task WriteAsyncImpl(byte[] buffer, FieldLength length, CancellationToken cancellationToken)
        {
            if (length == FieldLength.Zero)
            {
                return;
            }

            WriteCheck(length);

            if (Source is BoundedStream boundedStream && !IsByteBarrier)
            {
                await boundedStream.WriteAsync(buffer, length, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (length.BitCount == 0 && _bitOffset == 0)
                {
                    // trivial byte-aligned case, write to underlying stream
                    await WriteByteAlignedAsync(buffer, (int) length.ByteCount, cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    var lastByteIndex = length.BitCount == 0 ? length.ByteCount - 1 : length.ByteCount;
                    var bitCount = length.BitCount == 0 ? BitsPerByte : length.BitCount;

                    await WriteBitsAsync(buffer[lastByteIndex], bitCount, cancellationToken)
                        .ConfigureAwait(false);

                    // collect bits in this, the bottom bounded stream
                    for (long i = 0; i < lastByteIndex; i++)
                    {
                        await WriteBitsAsync(buffer[lastByteIndex - (i + 1)], BitsPerByte, cancellationToken)
                            .ConfigureAwait(false);
                    }
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

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
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

            var remaining = BitsPerByte - (count + _bitOffset);

            var shiftedValue = remaining > 0 ? value << remaining : value >> -remaining;
            _bitBuffer |= (byte) shiftedValue;
            _bitOffset += count;

            if (_bitOffset >= BitsPerByte)
            {
                var data = new[] {_bitBuffer};
                WriteByteAligned(data, data.Length);
                _bitBuffer = (byte) (value << (remaining + BitsPerByte));
            }

            _bitOffset %= BitsPerByte;
        }


        private async Task WriteBitsAsync(byte value, int count, CancellationToken cancellationToken)
        {
            if (count == 0)
            {
                return;
            }

            var remaining = BitsPerByte - (count + _bitOffset);

            var shiftedValue = remaining > 0 ? value << remaining : value >> -remaining;
            _bitBuffer |= (byte) shiftedValue;
            _bitOffset += count;

            if (_bitOffset >= BitsPerByte)
            {
                var data = new[] {_bitBuffer};
                await WriteByteAlignedAsync(data, data.Length, cancellationToken).ConfigureAwait(false);
                _bitBuffer = (byte) (value << (remaining + BitsPerByte));
            }

            _bitOffset %= BitsPerByte;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            var read = await ReadAsyncImpl(buffer, count, cancellationToken)
                .ConfigureAwait(false);
            return (int) read.ByteCount;
        }

        public FieldLength Read(byte[] buffer, FieldLength length)
        {
            return ReadImpl(buffer, length);
        }

        public Task<FieldLength> ReadAsync(byte[] buffer, FieldLength length, CancellationToken cancellationToken)
        {
            return ReadAsyncImpl(buffer, length, cancellationToken);
        }

        private FieldLength ReadImpl(byte[] buffer, FieldLength length)
        {
            length = ClampLength(length);

            FieldLength readLength;

            if (length == FieldLength.Zero)
            {
                return FieldLength.Zero;
            }

            if (Source is BoundedStream boundedStream && !IsByteBarrier)
            {
                readLength = boundedStream.Read(buffer, length);
            }
            else
            {
                if (length.BitCount == 0 && _bitOffset == 0)
                {
                    // trivial byte-aligned case
                    readLength = ReadByteAligned(buffer, (int) length.ByteCount);
                }
                else
                {
                    readLength = FieldLength.Zero;

                    var lastByteIndex = length.BitCount == 0 ? length.ByteCount - 1 : length.ByteCount;
                    var bitCount = length.BitCount == 0 ? BitsPerByte : length.BitCount;

                    var readBitCount = ReadBits(bitCount, out var value);

                    buffer[lastByteIndex] = value;
                    readLength += FieldLength.FromBitCount(readBitCount);

                    for (long i = 0; i < lastByteIndex; i++)
                    {
                        readBitCount = ReadBits(BitsPerByte, out value);
                        buffer[lastByteIndex - (i + 1)] = value;
                        readLength += FieldLength.FromBitCount(readBitCount);
                    }
                }
            }

            RelativePosition += readLength;
            return readLength;
        }

        private async Task<FieldLength> ReadAsyncImpl(byte[] buffer, FieldLength length,
            CancellationToken cancellationToken)
        {
            length = ClampLength(length);

            FieldLength readLength;

            if (length == FieldLength.Zero)
            {
                return FieldLength.Zero;
            }

            if (Source is BoundedStream boundedStream && !IsByteBarrier)
            {
                readLength = await boundedStream.ReadAsync(buffer, length, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (length.BitCount == 0 && _bitOffset == 0)
                {
                    // trivial byte-aligned case
                    readLength = await ReadByteAlignedAsync(buffer, (int) length.ByteCount, cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    readLength = FieldLength.Zero;

                    var lastByteIndex = length.BitCount == 0 ? length.ByteCount - 1 : length.ByteCount;
                    var bitCount = length.BitCount == 0 ? BitsPerByte : length.BitCount;

                    var value = new byte[1];
                    var readBitCount =
                        await ReadBitsAsync(bitCount, value, cancellationToken).ConfigureAwait(false);
                    buffer[lastByteIndex] = value[0];
                    readLength += FieldLength.FromBitCount(readBitCount);

                    for (long i = 0; i < lastByteIndex; i++)
                    {
                        readBitCount = await ReadBitsAsync(BitsPerByte, value, cancellationToken)
                            .ConfigureAwait(false);

                        buffer[lastByteIndex - (i + 1)] = value[0];
                        readLength += FieldLength.FromBitCount(readBitCount);
                    }
                }
            }

            RelativePosition += readLength;
            return readLength;
        }


        protected virtual int ReadByteAligned(byte[] buffer, int length)
        {
            return Source.Read(buffer, 0, length);
        }

        protected virtual Task<int> ReadByteAlignedAsync(byte[] buffer, int length, CancellationToken cancellationToken)
        {
            return Source.ReadAsync(buffer, 0, length, cancellationToken);
        }

        private int ReadBits(int count, out byte value)
        {
            if (count == 0)
            {
                value = 0;
                return 0;
            }

            value = _bitBuffer;

            if (count > _bitOffset)
            {
                var data = new byte[1];
                int read = ReadByteAligned(data, data.Length);

                if (read == 0)
                {
                    return 0;
                }

                _bitBuffer = data[0];
                value |= (byte)(_bitBuffer >> _bitOffset);
                _bitBuffer = (byte)(_bitBuffer << (count - _bitOffset));
                _bitOffset += BitsPerByte;
            }
            else
            {
                _bitBuffer = (byte)(_bitBuffer << count);
            }

            value = (byte)(value >> (BitsPerByte - count));

            _bitOffset -= count;

            return count;
        }

        private async Task<int> ReadBitsAsync(int count, byte[] value, CancellationToken cancellationToken)
        {
            if (count == 0)
            {
                return 0;
            }

            if (value.Length != 1)
            {
                throw new ArgumentException();
            }

            value[0] = _bitBuffer;

            if (count > _bitOffset)
            {
                var data = new byte[1];
                var read = await ReadByteAlignedAsync(data, data.Length, cancellationToken).ConfigureAwait(false);

                if (read == 0)
                {
                    return 0;
                }

                _bitBuffer = data[0];
                value[0] |= (byte)(_bitBuffer >> _bitOffset);
                _bitBuffer = (byte)(_bitBuffer << (count - _bitOffset));
                _bitOffset += BitsPerByte;
            }
            else
            {
                _bitBuffer = (byte)(_bitBuffer << count);
            }

            value[0] = (byte)(value[0] >> (BitsPerByte - count));

            _bitOffset -= count;

            return count;
        }

        private FieldLength ClampLength(FieldLength length)
        {
            if (MaxLength != null && length > MaxLength - Position)
            {
                length = FieldLength.Max(FieldLength.Zero, MaxLength - Position);
            }

            return length;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}