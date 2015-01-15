using System;
using System.Collections;
using System.IO;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitiveListValueNode : PrimitiveCollectionValueNode
    {
        public PrimitiveListValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void PrimitiveCollectionSerializeOverride(Stream stream)
        {
            var typeNode = (ListTypeNode)TypeNode;
            var dummyChild = (ValueValueNode)typeNode.Child.CreateSerializer(this);

            var writer = new EndianAwareBinaryWriter(stream, Endianness);
            var childSerializedType = dummyChild.TypeNode.GetSerializedType();

            var list = Value as IList;

            if (list == null)
                return;

            foreach (var value in list)
                dummyChild.Serialize(writer, value, childSerializedType);
        }

        protected override object CreateCollection(int size)
        {
            var typeNode = (ListTypeNode)TypeNode;
            var array = Array.CreateInstance(typeNode.ChildType, size);
            return Activator.CreateInstance(TypeNode.Type, array);
        }

        protected override void SetCollectionValue(object item, int index)
        {
            var list = (IList) Value;
            list[index] = item;
        }

        protected override long CountOverride()
        {
            var list = (IList) Value;
            return list.Count;
        }
    }
}
