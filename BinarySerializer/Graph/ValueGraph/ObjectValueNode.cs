using System.IO;
using System.Linq;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        public ObjectValueNode(Node parent) : base(parent)
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
