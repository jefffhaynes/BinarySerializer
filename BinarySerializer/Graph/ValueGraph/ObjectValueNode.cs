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

        public override object Value { get; set; }

        protected override void SerializeOverride(Stream stream)
        {
            foreach (var child in Children.Cast<ValueNode>())
            {
                child.Serialize(stream);
            }
        }

        public override void DeserializeOverride(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
