using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        public ObjectValueNode(Node parent, TypeNode typeNode)
            : base(parent, typeNode)
        {
        }

        protected override void SerializeOverride(Stream stream)
        {
            foreach (var child in Children.Cast<ValueNode>())
            {
                child.Serialize(stream);
            }
        }
    }
}
