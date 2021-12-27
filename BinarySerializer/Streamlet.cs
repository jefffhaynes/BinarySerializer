namespace BinarySerialization;

/// <summary>
///     A <see cref="Stream" /> that implements reading a section of a source <see cref="Stream" />.
///     The source <see cref="Stream" /> must support seeking.
/// </summary>
public sealed class Streamlet : Stream
{
    private long _length;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Streamlet" /> class with a source stream, an offset, and a length.
    /// </summary>
    /// <param name="source">
    ///     The source <see cref="Stream" />.
    /// </param>
    /// <param name="offset">
    ///     An offset to a position in the source <see cref="Stream" /> at which
    ///     this <see cref="Streamlet" /> will read from.
    /// </param>
    /// <param name="length">
    ///     The length of this <see cref="Streamlet" />.
    /// </param>
    public Streamlet(Stream source, long offset, long length)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "The offset cannot be negative");
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "The length cannot be negative");
        }

        if (offset + length > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "The length cannot exceed the source stream");
        }

        if (!source.CanSeek)
        {
            throw new ArgumentException("The source stream must support seeking", nameof(source));
        }

        Source = source;
        Offset = offset;
        _length = length;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Streamlet" /> class with a source stream, an offset.
    ///     Length is assumed to be the remainder of the source stream.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="offset"></param>
    public Streamlet(Stream source, long offset) : this(source, offset, source.Length - offset)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Streamlet" /> class with a source stream.
    ///     Offset is assumed to be the current position of the source stream.  Length is assumed to be the remainder of the
    ///     source stream.
    /// </summary>
    /// <param name="source"></param>
    public Streamlet(Stream source) : this(source, source.Position)
    {
    }

    /// <summary>
    ///     Gets a value indicating whether the source stream supports reading.
    /// </summary>
    public override bool CanRead => Source.CanRead;

    //  Gets a value indicating whether the source stream supports seeking.
    /// <summary>
    /// </summary>
    public override bool CanSeek => Source.CanSeek;

    /// <summary>
    ///     Gets a value indicating whether the current stream supports writing.  This always returns <c>false</c>.
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    ///     Gets the length in bytes of the <see cref="Streamlet" />.
    /// </summary>
    public override long Length => _length;

    /// <summary>
    ///     The underlying source <see cref="Stream" />.
    /// </summary>
    public Stream Source { get; }

    /// <summary>
    ///     Gets or sets the position within the <see cref="Streamlet" />.
    /// </summary>
    public override long Position { get; set; }

    /// <summary>
    ///     The offset in the source <see cref="Stream" /> at which this <see cref="Streamlet" /> will start.
    /// </summary>
    public long Offset { get; set; }

    /// <summary>
    ///     Does nothing since a <see cref="Streamlet" /> is read-only.
    /// </summary>
    public override void Flush()
    {
    }

    /// <summary>
    ///     Reads a block of bytes from the current stream and writes the data to a buffer.
    /// </summary>
    /// <param name="buffer">
    ///     When this method returns, contains the specified byte array with the
    ///     values between offset and (offset + count - 1) replaced by the characters read from the current stream.
    /// </param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin storing data from the current stream.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>
    ///     The total number of bytes written into the buffer. This can be less than the number of bytes requested
    ///     if that number of bytes are not currently available, or zero if the end of the stream is reached before any bytes
    ///     are read.
    /// </returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        count = ClampCount(count);

        if (count == 0)
        {
            return 0;
        }

        Source.Position = Offset + Position;

        var read = Source.Read(buffer, offset, count);
        Position += read;

        return read;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        count = ClampCount(count);

        if (count == 0)
        {
            return 0;
        }

        Source.Position = Offset + Position;

        var read = await Source.ReadAsync(buffer, offset, count, cancellationToken)
            .ConfigureAwait(false);
        Position += read;

        return read;
    }

    /// <summary>
    ///     Sets the position within the current stream to the specified value.
    /// </summary>
    /// <param name="offset">
    ///     The new position within the stream. This is relative to the origin parameter, and can be positive
    ///     or negative.
    /// </param>
    /// <param name="origin">A value of type SeekOrigin, which acts as the seek reference point.</param>
    /// <returns>The new position within the stream, calculated by combining the initial reference point and the offset.</returns>
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
    ///     Sets the length of the current stream to the specified value.
    /// </summary>
    /// <param name="value">The value at which to set the length.</param>
    public override void SetLength(long value)
    {
        _length = value;
    }

    /// <summary>
    ///     This method always throws a <see cref="NotSupportedException" />.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    ///     Closes the <see cref="Streamlet" /> object.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Source.Dispose();
    }

    private int ClampCount(int count)
    {
        if (count > Length - Position)
        {
            count = Math.Max(0, (int)(Length - Position));
        }
        return count;
    }
}
