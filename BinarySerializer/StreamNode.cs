using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinarySerialization
{
    internal class StreamNode : Node
    {
        public StreamNode(Node parent, Type type) : base(parent, type)
        {
        }

        public StreamNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        public override object BoundValue
        {
            get { throw new BindingException("Cannot bind to a stream."); }
        }

        protected override object OnMeasureNode()
        {
            var valueStream = (Stream) Value;

            if (FieldLengthEvaluator.IsConst)
                return FieldLengthEvaluator.Value;

            if (valueStream.CanSeek)
                return valueStream.Length;

            throw new InvalidOperationException("Cannot bind non-seekable stream.");
        }

        public override void Serialize(Stream stream)
        {
            var valueStream = (Stream) Value;

            var valueStreamlet = FieldLengthEvaluator.IsConst
                ? new Streamlet(valueStream, valueStream.Position, (long) FieldLengthEvaluator.Value)
                : new Streamlet(valueStream);

            valueStreamlet.CopyTo(stream);
        }

        public override void Deserialize(StreamLimiter stream)
        {
            /* This is weird but we need to find the base stream so we can reference it directly */
            Stream baseStream = stream;
            while (baseStream is StreamLimiter)
                baseStream = (baseStream as StreamLimiter).Source;

            Value = FieldLengthEvaluator != null
                ? new Streamlet(baseStream, baseStream.Position, (long)FieldLengthEvaluator.Value)
                : new Streamlet(baseStream, baseStream.Position);

            if (FieldLengthEvaluator != null)
                stream.Seek((long) FieldLengthEvaluator.Value, SeekOrigin.Current);
            else stream.Seek(0, SeekOrigin.End);
        }
    }
}
