using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class StreamTypeNode : TypeNode
    {
        public StreamTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public StreamTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new StreamValueNode(parent, Name, this);
        }
    }
}
