using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class EnumValueNode : ValueValueNode
    {
        public EnumValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var typeNode = (EnumTypeNode) TypeNode;
            var enumInfo = typeNode.EnumInfo;
            var value = enumInfo.EnumValues != null ? enumInfo.EnumValues[(Enum)BoundValue] : BoundValue;
            Serialize(stream, value, enumInfo.SerializedType, enumInfo.EnumValueLength);
        }

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var typeNode = (EnumTypeNode)TypeNode;
            var enumInfo = typeNode.EnumInfo;

            var value = Deserialize(stream, enumInfo.SerializedType, enumInfo.EnumValueLength);

            if (enumInfo.ValueEnums != null)
            {
                value = enumInfo.ValueEnums[(string)value];
            }

            Func<object, object> converter;
            var underlyingValue = TypeConverters.TryGetValue(enumInfo.UnderlyingType, out converter)
                ? converter(value)
                : value;

            Value = Enum.ToObject(TypeNode.BaseSerializedType, underlyingValue);
        }
    }
}
