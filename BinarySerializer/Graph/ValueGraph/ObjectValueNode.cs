using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        public ObjectValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }


        protected override void SerializeOverride(Stream stream)
        {
            foreach (var child in Children.Cast<ValueNode>())
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
