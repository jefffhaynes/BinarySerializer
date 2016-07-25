using System;
using System.IO;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class StreamValueNode : ValueNode
    {
        public StreamValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var valueStream = (Stream)Value;

            var length = GetConstFieldLength();

            if (length != null)
            {
                var valueStreamlet = new Streamlet(valueStream, valueStream.Position, length.Value);
                valueStreamlet.CopyTo(stream);
            }
            else
            {
                valueStream.CopyTo(stream);
            }
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            /* This is weird but we need to find the base stream so we can reference it directly */
            Stream baseStream = stream;
            while (baseStream is BoundedStream)
                baseStream = (baseStream as BoundedStream).Source;

            var length = GetFieldLength();

            Value = length != null
                ? new Streamlet(baseStream, baseStream.Position, length.Value)
                : new Streamlet(baseStream, baseStream.Position);

            if (length != null)
                stream.Seek(length.Value, SeekOrigin.Current);
            else stream.Seek(0, SeekOrigin.End);
        }

        protected override long MeasureOverride()
        {
            var valueStream = (Stream)Value;

            var length = GetConstFieldLength();

            if (length != null)
                return length.Value;
            
            if (valueStream.CanSeek)
                return valueStream.Length;

            throw new InvalidOperationException("Cannot bind non-seekable stream.");
        }
    }
}
