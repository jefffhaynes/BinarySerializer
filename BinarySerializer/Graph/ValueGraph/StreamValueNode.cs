namespace BinarySerialization.Graph.ValueGraph;

internal class StreamValueNode : ValueNode
{
    private const int CopyToBufferSize = 81920;

    public StreamValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
    {
    }

    public override object Value { get; set; }

    internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
    {
        var valueStream = (Stream)Value;

        var length = GetConstFieldLength();

        if (length != null)
        {
            var valueStreamlet = new Streamlet(valueStream, valueStream.Position, length.ByteCount);
            valueStreamlet.CopyTo(stream);
        }
        else
        {
            valueStream.CopyTo(stream);
        }
    }

    internal override async Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
    {
        var valueStream = (Stream)Value;

        var length = GetConstFieldLength();

        if (length != null)
        {
            var valueStreamlet = new Streamlet(valueStream, valueStream.Position, length.ByteCount);
            await valueStreamlet.CopyToAsync(stream, CopyToBufferSize, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await valueStream.CopyToAsync(stream, CopyToBufferSize, cancellationToken).ConfigureAwait(false);
        }
    }

    internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
    {
        var rootStream = GetRootStream(stream);

        var length = GetFieldLength();

        Value = length != null
            ? new Streamlet(rootStream, rootStream.Position, length.ByteCount)
            : new Streamlet(rootStream, rootStream.Position);

        if (length != null)
        {
            var nullStream = new NullStream();
            stream.CopyTo(nullStream, (int)length.ByteCount, CopyToBufferSize);
        }
        else
        {
            stream.Seek(0, SeekOrigin.End);
        }
    }

    internal override async Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
        CancellationToken cancellationToken)
    {
        var rootStream = GetRootStream(stream);

        var length = GetFieldLength();

        Value = length != null
            ? new Streamlet(rootStream, rootStream.Position, length.ByteCount)
            : new Streamlet(rootStream, rootStream.Position);

        if (length != null)
        {
            var nullStream = new NullStream();
            await stream.CopyToAsync(nullStream, (int)length.ByteCount, CopyToBufferSize, cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            stream.Seek(0, SeekOrigin.End);
        }
    }

    protected override FieldLength MeasureOverride()
    {
        var valueStream = (Stream)Value;

        var length = GetConstFieldLength();

        if (length != null)
        {
            return length;
        }

        if (valueStream.CanSeek)
        {
            return valueStream.Length;
        }

        throw new InvalidOperationException("Cannot bind non-seekable stream.");
    }

    private static Stream GetRootStream(BoundedStream stream)
    {
        Stream baseStream = stream;
        while (baseStream is BoundedStream boundedStream)
        {
            baseStream = boundedStream.Source;
        }

        return baseStream;
    }
}
