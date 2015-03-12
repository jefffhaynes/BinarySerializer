using System;
using System.Collections;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitiveListValueNode : PrimitiveCollectionValueNode
    {
        public PrimitiveListValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void PrimitiveCollectionSerializeOverride(StreamLimiter stream, int? itemLength, int? itemCount)
        {
            var typeNode = (ListTypeNode)TypeNode;
            var childSerializer = (ValueValueNode)typeNode.Child.CreateSerializer(this);

            var writer = new EndianAwareBinaryWriter(stream, Endianness);
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var list = Value as IList;

            if (list == null)
                return;

            // Handle const-sized mismatched collections
            if (itemCount != null && list.Count != itemCount)
            {
                var tempList = list;
                list = (IList) CreateCollection(itemCount.Value);

                for (int i = 0; i < Math.Min(tempList.Count, list.Count); i++)
                    list[i] = tempList[i];
            }

            foreach (var value in list)
            {
                if (stream.IsAtLimit)
                    break;

                childSerializer.Serialize(writer, value, childSerializedType, itemLength);
            }
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
