using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class PrimitiveCollectionValueNode : ValueNode
    {
        protected PrimitiveCollectionValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }


        public override object BoundValue
        {
            get { throw new NotImplementedException(); }
        }

        protected override void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            IEnumerable<int> itemLengths = null;
            if (TypeNode.ItemLengthBinding != null && TypeNode.ItemLengthBinding.IsConst)
            {
                var constValue = TypeNode.ItemLengthBinding.ConstValue;

                var constEnumerable = constValue as IEnumerable;
                if (constEnumerable != null)
                {
                    itemLengths = constEnumerable.Cast<object>().Select(Convert.ToInt32);
                }
                else
                {
                    var itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.ConstValue);
                    itemLengths = DefaultItemLengthSource(itemLength);
                }
            }

            int? itemCount = null;
            if (TypeNode.FieldCountBinding != null && TypeNode.FieldCountBinding.IsConst)
                itemCount = Convert.ToInt32(TypeNode.FieldCountBinding.ConstValue);

            PrimitiveCollectionSerializeOverride(stream, itemLengths, itemCount);

            var typeNode = (CollectionTypeNode)TypeNode;

            /* Add termination */
            if (typeNode.TerminationChild != null)
            {
                var terminationChild = typeNode.TerminationChild.CreateSerializer(this);
                terminationChild.Value = typeNode.TerminationValue;
                terminationChild.Serialize(stream, eventShuttle);
            }
        }

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            /* Create temporary list */
            Type collectionType = typeof(List<>).MakeGenericType(typeNode.ChildType);
            var collection = (IList)Activator.CreateInstance(collectionType);

            /* Create single serializer to do all the work */
            var childSerializer = (ValueValueNode)typeNode.Child.CreateSerializer(this);

            var reader = new EndianAwareBinaryReader(stream, Endianness);
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var count = TypeNode.FieldCountBinding != null ? Convert.ToInt32(TypeNode.FieldCountBinding.GetValue(this)) : int.MaxValue;

            int? itemLength = null;
            if (TypeNode.ItemLengthBinding != null)
                itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.GetValue(this));

            var terminationValue = typeNode.TerminationValue;
            var terminationChild = typeNode.TerminationChild == null ? null : typeNode.TerminationChild.CreateSerializer(this);

            int itemCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                /* Check termination case */
                if (terminationChild != null)
                {
                    using (var streamResetter = new StreamResetter(stream))
                    {
                        terminationChild.Deserialize(stream, eventShuttle);

                        if (terminationChild.Value.Equals(terminationValue))
                        {
                            streamResetter.CancelReset();
                            break;
                        }
                    }
                }

                var value = childSerializer.Deserialize(reader, childSerializedType, itemLength);
                collection.Add(value);

                itemCount++;
            }

            /* Create final collection */
            Value = CreateCollection(itemCount);

            /* Copy temp list into final collection */
            for (int i = 0; i < itemCount; i++)
                SetCollectionValue(collection[i], i);
        }

        protected abstract void PrimitiveCollectionSerializeOverride(StreamLimiter stream, IEnumerable<int> itemLengths, int? itemCount);

        protected abstract object CreateCollection(int size);

        protected abstract void SetCollectionValue(object item, int index);


        protected override object GetLastItemValueOverride()
        {
            throw new InvalidOperationException("Not supported on primitive collections.  Use SerializeUntil attribute.");
        }

        private static IEnumerable<int> DefaultItemLengthSource(int length)
        {
            while (true)
                yield return length;
        }
    }
}
