using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerialization.ValueGraph
{
    public class ValueGraphObjectNode : ValueGraphNode
    {
        public ValueGraphObjectNode()
        {
        }

        public ValueGraphObjectNode(IEnumerable<ValueGraphNode> children) : base(children)
        {
        }

        protected override void SerializeOverride(Stream stream)
        {
            foreach (var child in Children)
            {
                child.Serialize(stream);
            }
        }
    }
}
