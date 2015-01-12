using System;
using System.IO;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitveArrayValueNode : PrimitiveCollectionValueNode
    {
        public PrimitveArrayValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }


        protected override void SerializeOverride(Stream stream)
        {
            var typeNode = (ArrayTypeNode)TypeNode;
            var dummyChild = (ValueValueNode) typeNode.Child.CreateSerializer(this);

            var writer = new EndianAwareBinaryWriter(stream, Endianness);
            var childSerializedType = dummyChild.TypeNode.GetSerializedType();

            var array = (Array) Value;
            for (int i = 0; i < array.Length; i++)
            {
                var value = array.GetValue(i);
                dummyChild.Serialize(writer, value, childSerializedType);
            }
        }

        protected override object CreateCollection(int size)
        {
            var typeNode = (ArrayTypeNode)TypeNode;
            return Array.CreateInstance(typeNode.ChildType, size);
        }

        protected override void SetCollectionValue(object item, int index)
        {
            var array = (Array)Value;
            array.SetValue(item, index);
        }
    }
}
