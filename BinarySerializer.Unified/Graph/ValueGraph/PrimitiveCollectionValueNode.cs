using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class PrimitiveCollectionValueNode : ValueNode
    {
        protected PrimitiveCollectionValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }

        public override object BoundValue
        {
            get
            {
                object value;

                if (Bindings.Count > 0)
                {
                    value = Bindings[0]();

                    var enumerableValue = value as IEnumerable;

                    if (enumerableValue == null)
                        throw new InvalidOperationException("Complex types cannot be binding sources for scalar values.");

                    if (Bindings.Count != 1)
                    {
                        var bindingValues = Bindings.Select(binding => binding() as IEnumerable).ToList();

                        if (bindingValues.Any(o => o == null))
                            throw new InvalidOperationException(
                                "Complex types cannot be binding sources for scalar values.");

                        if (bindingValues.Select(enumerable => enumerable.Cast<object>())
                            .Any(enumerable => !enumerable.SequenceEqual(enumerableValue.Cast<object>())))
                        {
                            throw new BindingException(
                                "Multiple bindings to a single source must have equivalent target values.");
                        }
                    }

                    value = CreateCollection(enumerableValue);
                }
                else value = Value;

                return value;
            }
        }

        internal override void SerializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            int? itemLength = null;
            if (TypeNode.ItemLengthBinding != null && TypeNode.ItemLengthBinding.IsConst)
                itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.ConstValue);

            int? itemCount = null;
            if (TypeNode.FieldCountBinding != null && TypeNode.FieldCountBinding.IsConst)
                itemCount = Convert.ToInt32(TypeNode.FieldCountBinding.ConstValue);

            PrimitiveCollectionSerializeOverride(stream, itemLength, itemCount);

            var typeNode = (CollectionTypeNode) TypeNode;

            /* Add termination */
            if (typeNode.TerminationChild != null)
            {
                var terminationChild = typeNode.TerminationChild.CreateSerializer(this);
                terminationChild.Value = typeNode.TerminationValue;
                terminationChild.Serialize(stream, eventShuttle);
            }
        }

        internal override void DeserializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            var items =
                DeserializeCollection(stream, eventShuttle).ToList();

            var itemCount = items.Count;

            /* Create final collection */
            Value = CreateCollection(itemCount);

            /* Copy temp list into final collection */
            for (var i = 0; i < itemCount; i++)
                SetCollectionValue(items[i], i);
        }

        private IEnumerable<object> DeserializeCollection(LimitedStream stream, EventShuttle eventShuttle)
        {
            var typeNode = (CollectionTypeNode) TypeNode;

            /* Create single serializer to do all the work */
            var childSerializer = (ValueValueNode) typeNode.Child.CreateSerializer(this);

            var reader = new EndianAwareBinaryReader(stream, Endianness);
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var count = TypeNode.FieldCountBinding != null
                ? Convert.ToInt32(TypeNode.FieldCountBinding.GetValue(this))
                : int.MaxValue;

            int? itemLength = null;
            if (TypeNode.ItemLengthBinding != null)
                itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.GetValue(this));

            var terminationValue = typeNode.TerminationValue;
            var terminationChild = typeNode.TerminationChild?.CreateSerializer(this);

            for (var i = 0; i < count; i++)
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

                yield return childSerializer.Deserialize(reader, childSerializedType, itemLength);
            }
        }

        protected abstract void PrimitiveCollectionSerializeOverride(LimitedStream stream, int? length, int? itemCount);
        protected abstract object CreateCollection(int size);
        protected abstract object CreateCollection(IEnumerable enumerable);
        protected abstract void SetCollectionValue(object item, int index);

        protected override object GetLastItemValueOverride()
        {
            throw new InvalidOperationException("Not supported on primitive collections.  Use SerializeUntil attribute.");
        }
    }
}