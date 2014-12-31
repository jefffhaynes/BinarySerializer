using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinarySerialization.Graph
{
    internal class CustomNode : Node
    {
        public CustomNode(Node parent) : base(parent)
        {
        }

        public CustomNode(Node parent, Type type) : base(parent, type)
        {
        }

        public CustomNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        public override void SerializeOverride(Stream stream)
        {
            var serializationContext = CreateSerializationContext();
            var value = (IBinarySerializable) Value;
            value.Serialize(stream, Endianness, serializationContext);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            var serializationContext = CreateSerializationContext();
            var value = (IBinarySerializable) Value;
            value.Deserialize(stream, Endianness, serializationContext);
        }
    }
}
