using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class RootTypeNode : ContainerTypeNode
    {
        private readonly TypeNode _child;
        private object _context;
        private List<TypeNode> _contextChildren; 

        public RootTypeNode(Type graphType) : base(null, graphType)
        {
            Child = GenerateChild(graphType);
        }

        public override Type Type
        {
            get { return _context != null ? _context.GetType() : null; }
        }

        //public override IEnumerable<Node> Children
        //{
        //    get
        //    {
        //        var child = new[] { _child };
        //        if (_contextChildren == null && _child == null)
        //            return Enumerable.Empty<Node>();

        //        if (_contextChildren == null)
        //            return child;

        //        if (_child == null)
        //            return _contextChildren;

        //        return _contextChildren.Union(child);
        //    }
        //}

        public TypeNode Child { get; private set; }

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

        //private IEnumerable<TypeNode> GenerateChildrenImpl(Type type)
        //{
        //    IEnumerable<MemberInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        //    IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        //    IEnumerable<MemberInfo> all = properties.Union(fields);

        //    return all.Select(GenerateChild);
        //}

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ContextNode(parent, Name, this);
        }

        //public override object DeserializeOverride(ValueNode node)
        //{
        //    return _child.Deserialize(node);
        //}

        public override ValueNode CreateSerializer(ValueNode parent)
        {
            return CreateSerializerOverride(parent);
        }

        //public override object Deserialize(ValueNode node)
        //{
        //    return DeserializeOverride(node);
        //}

        public override Endianness Endianness { get; set; }
    }
}
