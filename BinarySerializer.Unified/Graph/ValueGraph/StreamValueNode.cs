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

        internal override void SerializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            var valueStream = (Stream)Value;

            var valueStreamlet = TypeNode.FieldLengthBinding.IsConst
                ? new Streamlet(valueStream, valueStream.Position, Convert.ToInt64(TypeNode.FieldLengthBinding.ConstValue))
                : new Streamlet(valueStream);

            valueStreamlet.CopyTo(stream);
        }

        internal override void DeserializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            /* This is weird but we need to find the base stream so we can reference it directly */
            Stream baseStream = stream;
            while (baseStream is LimitedStream)
                baseStream = (baseStream as LimitedStream).Source;

            var length = TypeNode.FieldLengthBinding == null
                ? (long?)null
                : Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this));

            Value = length != null
                ? new Streamlet(baseStream, baseStream.Position, Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this)))
                : new Streamlet(baseStream, baseStream.Position);

            if (length != null)
                stream.Seek(length.Value, SeekOrigin.Current);
            else stream.Seek(0, SeekOrigin.End);
        }

        protected override long MeasureOverride()
        {
            var valueStream = (Stream)Value;

            if (TypeNode.FieldLengthBinding.IsConst)
                return Convert.ToInt64(TypeNode.FieldLengthBinding.ConstValue);

            if (valueStream.CanSeek)
                return valueStream.Length;

            throw new InvalidOperationException("Cannot bind non-seekable stream.");
        }
    }
}
