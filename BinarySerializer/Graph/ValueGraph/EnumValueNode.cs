using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class EnumValueNode : ValueValueNode
    {
        public EnumValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var typeNode = (EnumTypeNode) TypeNode;
            var enumInfo = typeNode.EnumInfo;
            var value = enumInfo.EnumValues != null ? enumInfo.EnumValues[(Enum)BoundValue] : BoundValue;
            Serialize(stream, value, enumInfo.SerializedType, enumInfo.EnumValueLength);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var typeNode = (EnumTypeNode)TypeNode;
            var enumInfo = typeNode.EnumInfo;

            Deserialize(stream, enumInfo.SerializedType, enumInfo.EnumValueLength);
            var value = GetValue(enumInfo.SerializedType);

            if (enumInfo.ValueEnums != null)
            {
                value = enumInfo.ValueEnums[(string)value];
            }

            var underlyingValue = value.ConvertTo(enumInfo.UnderlyingType);

            Value = Enum.ToObject(TypeNode.BaseSerializedType, underlyingValue);
        }
    }
}
