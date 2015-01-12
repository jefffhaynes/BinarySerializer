using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class CollectionValueNode : ValueNode
    {
        protected CollectionValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        private IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Cast<ValueNode>().Where(child => child.TypeNode.IgnoreAttribute == null);
        }


        protected override void SerializeOverride(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
            {
                child.Serialize(stream);
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            foreach (var child in Children.Cast<ValueNode>())
            {
                child.Deserialize(stream);
            }
        }
    }
}
