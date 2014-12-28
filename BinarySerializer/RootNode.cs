using System;
using System.IO;

namespace BinarySerialization
{
    internal class RootNode : ContainerNode
    {
        private readonly Node _child;

        public RootNode(Type type) : base(null, type)
        {
            var child = GenerateChild(type);
            Children.Add(child);
            _child = child;
        }

        public override object Value
        {
            get { return _child.Value; }
            set { _child.Value = value; }
        }

        public override void SerializeOverride(Stream stream)
        {
            _child.Serialize(stream);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            _child.Deserialize(stream);
        }
    }
}
