using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal sealed class RootNode : ContainerNode
    {
        private readonly Node _child;
        private object _context;
        private List<Node> _contextChildren; 

        public RootNode(Type graphType) : base(null, graphType)
        {
            var child = GenerateChild(graphType);
            _child = child;
            AddChild(child);
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

        public override Type Type
        {
            get { return _context != null ? _context.GetType() : null; }
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                var child = new[] { _child };
                if (_contextChildren == null && _child == null)
                    return Enumerable.Empty<Node>();

                if (_contextChildren == null)
                    return child;

                if (_child == null)
                    return _contextChildren;

                return _contextChildren.Union(child);
            }
        }

        public object SerializationContext
        {
            get
            {
                return _context;
            }

            set
            {
                _contextChildren = null;

                _context = value;

                if (value != null)
                {
                    var children = GenerateChildrenImpl(value.GetType());
                    _contextChildren = new List<Node>(children);

                    foreach (var child in _contextChildren)
                        child.Value = child.ValueGetter(value);
                }
            }
        }

        private IEnumerable<Node> GenerateChildrenImpl(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            return all.Select(GenerateChild);
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
