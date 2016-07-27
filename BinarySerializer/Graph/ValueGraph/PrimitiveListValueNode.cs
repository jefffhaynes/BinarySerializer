using System;
using System.Collections;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitiveListValueNode : PrimitiveCollectionValueNode
    {
        public PrimitiveListValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void PrimitiveCollectionSerializeOverride(BoundedStream stream, long? itemLength, long? itemCount)
        {
            var typeNode = (ListTypeNode)TypeNode;
            var childSerializer = (ValueValueNode)typeNode.Child.CreateSerializer(this);

            var writer = new EndianAwareBinaryWriter(stream, GetFieldEndianness());
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var list = BoundValue as IList;

            if (list == null)
                return;

            // Handle const-sized mismatched collections
            if (itemCount != null && list.Count != itemCount)
            {
                var tempList = list;
                list = (IList)CreateCollection(itemCount.Value);

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

        protected override object CreateCollection(long size)
        {
            var typeNode = (ListTypeNode)TypeNode;
            var array = Array.CreateInstance(typeNode.ChildType, (int)size);
            return Activator.CreateInstance(typeNode.Type, array);
        }

        protected override object CreateCollection(IEnumerable enumerable)
        {
            var typeNode = (ListTypeNode)TypeNode;
            return enumerable.Cast<object>().Select(item => item.ConvertTo(typeNode.ChildType)).ToList();
        }

        protected override void SetCollectionValue(object item, long index)
        {
            var list = (IList) Value;
            var typeNode = (ListTypeNode)TypeNode;
            list[(int)index] = item.ConvertTo(typeNode.ChildType);
        }

        protected override long CountOverride()
        {
            var list = (IList) Value;

            if (list == null)
                return 0;

            return list.Count;
        }
    }
}
