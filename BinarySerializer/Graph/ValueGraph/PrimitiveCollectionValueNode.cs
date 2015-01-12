using System;
using System.Collections;
using System.Collections.Generic;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class PrimitiveCollectionValueNode : ValueNode
    {
        protected PrimitiveCollectionValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }


        public override void DeserializeOverride(StreamLimiter stream)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            /* Create temporary list */
            Type collectionType = typeof(List<>).MakeGenericType(typeNode.ChildType);
            var collection = (IList)Activator.CreateInstance(collectionType);

            /* Create single serializer to do all the work */
            var dummyChild = (ValueValueNode)typeNode.Child.CreateSerializer(this);

            var reader = new EndianAwareBinaryReader(stream, Endianness);
            var childSerializedType = dummyChild.TypeNode.GetSerializedType();

            var count = TypeNode.FieldCountBinding != null ? (int)TypeNode.FieldCountBinding.GetValue(this) : int.MaxValue;

            int itemCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                var value = dummyChild.Deserialize(reader, childSerializedType);
                collection.Add(value);

                itemCount++;
            }

            /* Create final collection */
            Value = CreateCollection(itemCount);

            /* Copy temp list into final collection */
            for (int i = 0; i < itemCount; i++)
            {
                SetCollectionValue(collection[i], i);
            }
        }


        protected abstract object CreateCollection(int size);

        protected abstract void SetCollectionValue(object item, int index);
    }
}
