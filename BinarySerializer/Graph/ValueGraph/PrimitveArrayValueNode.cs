using System;
using System.Collections;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PrimitveArrayValueNode : PrimitiveCollectionValueNode
    {
        public PrimitveArrayValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void PrimitiveCollectionSerializeOverride(BoundedStream stream, object boundValue,
            ValueValueNode childSerializer, SerializedType childSerializedType, long? itemLength, long? itemCount)
        {
            var array = (Array) BoundValue;

            // Handle const-sized mismatched collections
            if (itemCount != null && array.Length != itemCount)
            {
                var tempArray = array;
                array = (Array) CreateCollection(itemCount.Value);
                Array.Copy(tempArray, array, Math.Min(tempArray.Length, array.Length));
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (stream.IsAtLimit)
                    break;

                var value = array.GetValue(i);
                childSerializer.Serialize(stream, value, childSerializedType, itemLength);
            }
        }

        protected override object CreateCollection(long size)
        {
            var typeNode = (ArrayTypeNode)TypeNode;
            return Array.CreateInstance(typeNode.ChildType, (int)size);
        }

        protected override object CreateCollection(IEnumerable enumerable)
        {
            var typeNode = (ArrayTypeNode)TypeNode;
            return enumerable.Cast<object>().Select(item => item.ConvertTo(typeNode.ChildType)).ToArray();
        }

        protected override void SetCollectionValue(object item, long index)
        {
            var array = (Array)BoundValue;
            var typeNode = (ArrayTypeNode)TypeNode;
            array.SetValue(item.ConvertTo(typeNode.ChildType), (int)index);
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
