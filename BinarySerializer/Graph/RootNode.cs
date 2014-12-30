using System;
using System.IO;

namespace BinarySerialization.Graph
{
    internal class RootNode : ContainerNode
    {
        private readonly Node _child;

        public RootNode(Type type) : base(null, type)
        {
            var child = GenerateChild(type);
            AddChild(child);
            _child = child;
        }

        public override object Value
        {
            get { return _child.Value; }
            set { _child.Value = value; }
        }

        public override object BoundValue
        {
            get { return _child.BoundValue; }
        }

        public override void SerializeOverride(Stream stream)
        {
            _child.Serialize(stream);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            _child.Deserialize(stream);
        }

        public override void Serialize(Stream stream)
        {
            SerializeOverride(stream);
        }

        public override void Deserialize(StreamLimiter stream)
        {
            DeserializeOverride(stream);
        }

        public override Endianness Endianness { get; set; }
    }
}
