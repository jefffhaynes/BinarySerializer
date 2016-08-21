using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class DelegateValueNode : ValueNode
    {
        public DelegateValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            throw new NotImplementedException();
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            throw new NotImplementedException();
        }
    }
}
