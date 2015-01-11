using System;
using System.Collections.Generic;
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

        public override object Value
        {
            get
            {
                var value = Activator.CreateInstance(TypeNode.Type);

                var serializableChildren = GetSerializableChildren();

                foreach (var child in serializableChildren)
                    child.TypeNode.ValueSetter(value, child.Value);

                return value;
            }

            set
            {
                foreach (var child in Children.Cast<ValueNode>())
                    child.Value = child.TypeNode.ValueGetter(value);
            }
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
