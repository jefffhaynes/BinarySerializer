using System;
using System.Collections.Generic;
using System.Text;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class BitFieldValueNode : ValueValueNode
    {
        public BitFieldValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }
    }
}
