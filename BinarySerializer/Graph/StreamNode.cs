using System;
using System.IO;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal class StreamNode : Node
    {
        public StreamNode(Node parent, Type type) : base(parent, type)
        {
        }

        public StreamNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        protected override long MeasureNodeOverride()
        {
            var valueStream = (Stream) Value;

            if (FieldLengthBinding.IsConst)
                return (long)FieldLengthBinding.Value;

            if (valueStream.CanSeek)
                return valueStream.Length;

            throw new InvalidOperationException("Cannot bind non-seekable stream.");
        }

        public override void SerializeOverride(Stream stream)
        {
            var valueStream = (Stream) Value;

            var valueStreamlet = FieldLengthBinding.IsConst
                ? new Streamlet(valueStream, valueStream.Position, (long)FieldLengthBinding.Value)
                : new Streamlet(valueStream);

            valueStreamlet.CopyTo(stream);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            /* This is weird but we need to find the base stream so we can reference it directly */
            Stream baseStream = stream;
            while (baseStream is StreamLimiter)
                baseStream = (baseStream as StreamLimiter).Source;

            Value = FieldLengthBinding != null
                ? new Streamlet(baseStream, baseStream.Position, (long)FieldLengthBinding.Value)
                : new Streamlet(baseStream, baseStream.Position);

            if (FieldLengthBinding != null)
                stream.Seek((long) FieldLengthBinding.Value, SeekOrigin.Current);
            else stream.Seek(0, SeekOrigin.End);
        }
    }
}
