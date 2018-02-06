using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class StreamValueNode : ValueNode
    {
        private const int CopyToBufferSize = 81920;

        public StreamValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var valueStream = (Stream) Value;

            var length = GetConstFieldLength();

            if (length != null)
            {
                var valueStreamlet = new Streamlet(valueStream, valueStream.Position, (long) length.ByteCount);
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
                var valueStreamlet = new Streamlet(valueStream, valueStream.Position, (long) length.ByteCount);
                await valueStreamlet.CopyToAsync(stream, CopyToBufferSize, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await valueStream.CopyToAsync(stream, CopyToBufferSize,cancellationToken).ConfigureAwait(false);
            }
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var rootStream = GetRootStream(stream);

            var length = GetFieldLength();

            Value = length != null
                ? new Streamlet(rootStream, rootStream.Position, (long) length.ByteCount)
                : new Streamlet(rootStream, rootStream.Position);

            if (length != null)
            {
                var nullStream = new NullStream();
                stream.CopyTo(nullStream, (int) length.ByteCount, CopyToBufferSize);
            }
            else
            {
                stream.Seek(0, SeekOrigin.End);
            }
        }

        internal override Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            DeserializeOverride(stream, eventShuttle);
            return Task.CompletedTask;
        }

        protected override FieldLength MeasureOverride()
        {
            var valueStream = (Stream) Value;

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
            while (baseStream is BoundedStream)
            {
                baseStream = (baseStream as BoundedStream).Source;
            }
            return baseStream;
        }
    }
}