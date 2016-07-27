using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class CustomValueNode : ObjectValueNode
    {
        public CustomValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void ObjectSerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateLazySerializationContext();

            var value = BoundValue;

            if (value == null)
                return;

            var binarySerializable = value as IBinarySerializable;

            if (binarySerializable == null)
                throw new InvalidOperationException("Must implement IBinarySerializable");

            binarySerializable.Serialize(stream, GetFieldEndianness(), serializationContext);
        }

        protected override void ObjectDeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateLazySerializationContext();
            var binarySerializable = (IBinarySerializable)Activator.CreateInstance(TypeNode.Type);
            binarySerializable.Deserialize(stream, GetFieldEndianness(), serializationContext);
            Value = binarySerializable;
        }

        public override bool Visited
        {
            get { return base.Visited; }

            set
            {
                // no children will be marked as visited so we'll do it for them
                foreach (ValueNode child in Children)
                    child.Visited = value;

                base.Visited = value;
            }
        }
    }
}
