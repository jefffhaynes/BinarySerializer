using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitiveListValueNode : PrimitiveCollectionValueNode
    {
        public PrimitiveListValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void PrimitiveCollectionSerializeOverride(StreamLimiter stream, IEnumerable<int> itemLengths, int? itemCount)
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

            IEnumerator<int> itemLengthEnumerator = null;

            try
            {
                if (itemLengths != null)
                    itemLengthEnumerator = itemLengths.GetEnumerator();

                foreach (var value in list)
                {
                    if (stream.IsAtLimit)
                        break;

                    if (itemLengthEnumerator != null)
                        itemLengthEnumerator.MoveNext();

                    childSerializer.Serialize(writer, value, childSerializedType,
                        itemLengthEnumerator != null ? itemLengthEnumerator.Current : (int?)null);
                }

            }
            finally
            {
                if (itemLengthEnumerator != null)
                    itemLengthEnumerator.Dispose();
            }
        }

        protected override object CreateCollection(int size)
        {
            var typeNode = (ListTypeNode)TypeNode;
            var array = Array.CreateInstance(typeNode.ChildType, size);
            return Activator.CreateInstance(typeNode.Type, array);
        }

        protected override object CreateCollection(IEnumerable enumerable)
        {
            var typeNode = (ArrayTypeNode)TypeNode;
            return enumerable.Cast<object>().Select(item => ConvertToType(item, typeNode.ChildType)).ToList();
        }

        protected override void SetCollectionValue(object item, int index)
        {
            var list = (IList) Value;
            list[index] = item;
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
