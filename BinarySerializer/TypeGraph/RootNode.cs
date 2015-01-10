using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerialization.ValueGraph;

namespace BinarySerialization.TypeGraph
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
                //_contextChildren = null;

                //_context = value;

                //if (value != null)
                //{
                //    var children = GenerateChildrenImpl(value.GetType());
                //    _contextChildren = new List<Node>(children);

                //    foreach (var child in _contextChildren)
                //        child.Value = child.ValueGetter(value);
                //}
            }
        }

        private IEnumerable<Node> GenerateChildrenImpl(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            return all.Select(GenerateChild);
        }

        public override ValueGraphNode SerializeOverride(object value)
        {
            return _child.Serialize(value);
        }

        public override object DeserializeOverride(ValueGraphNode node)
        {
            return _child.Deserialize(node);
        }

        public override ValueGraphNode Serialize(object value)
        {
            return SerializeOverride(value);
        }

        public override object Deserialize(ValueGraphNode node)
        {
            return DeserializeOverride(node);
        }

        public override Endianness Endianness { get; set; }
    }
}
