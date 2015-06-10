using System;
using System.Collections.Generic;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitveArrayValueNode : PrimitiveCollectionValueNode
    {
        public PrimitveArrayValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void PrimitiveCollectionSerializeOverride(StreamLimiter stream, IEnumerable<int> itemLengths,
            int? itemCount)
        {
            var typeNode = (ArrayTypeNode) TypeNode;
            var childSerializer = (ValueValueNode) typeNode.Child.CreateSerializer(this);

            var writer = new EndianAwareBinaryWriter(stream, Endianness);
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var array = BoundValue as Array;

            if (array == null)
                return;

            // Handle const-sized mismatched collections
            if (itemCount != null && array.Length != itemCount)
            {
                var tempArray = array;
                array = (Array) CreateCollection(itemCount.Value);
                Array.Copy(tempArray, array, Math.Min(tempArray.Length, array.Length));
            }

            using (var itemLengthEnumerator = itemLengths.GetEnumerator())
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (stream.IsAtLimit)
                        break;

                    var value = array.GetValue(i);
                    itemLengthEnumerator.MoveNext();
                    childSerializer.Serialize(writer, value, childSerializedType, itemLengthEnumerator.Current);
                }
            }
        }

        protected override object CreateCollection(int size)
        {
            var typeNode = (ArrayTypeNode)TypeNode;
            return Array.CreateInstance(typeNode.ChildType, size);
        }

        protected override void SetCollectionValue(object item, int index)
        {
            var array = (Array)BoundValue;
            array.SetValue(item, index);
        }

        protected override long CountOverride()
        {
            var array = (Array)BoundValue;

            if (array == null)
                return 0;

            return array.Length;
        }
    }
}
