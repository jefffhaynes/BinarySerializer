using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class CustomValueNode : ObjectValueNode
    {
        //private IBinarySerializable _binarySerializable;

        public CustomValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        //public override object Value
        //{
        //    get { return base.Value; }

        //    set
        //    {
        //        var binarySerializable = value as IBinarySerializable;

        //        if(value != null && binarySerializable == null)
        //            throw new InvalidOperationException("Must implement IBinarySerializable");

        //        base.Value = binarySerializable;
        //    }
        //}

        protected override void ObjectSerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();

            var value = BoundValue;

            if (value == null)
                return;

            var binarySerializable = value as IBinarySerializable;

            if (binarySerializable == null)
                throw new InvalidOperationException("Must implement IBinarySerializable");

            binarySerializable.Serialize(stream, Endianness, serializationContext);
        }

        protected override void ObjectDeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializationContext = CreateSerializationContext();
            var binarySerializable = (IBinarySerializable)Activator.CreateInstance(TypeNode.Type);
            binarySerializable.Deserialize(stream, Endianness, serializationContext);
            Value = binarySerializable;
        }
    }
}
