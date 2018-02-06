using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class PrimitiveCollectionValueNode : CollectionValueNodeBase
    {
        protected PrimitiveCollectionValueNode(ValueNode parent, string name, TypeNode typeNode)
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
                    value = Bindings[0].Invoke();

                    if (!(value is IEnumerable enumerableValue))
                    {
                        throw new InvalidOperationException(
                            "Complex types cannot be binding sources for scalar values.");
                    }

                    if (Bindings.Count != 1)
                    {
                        var bindingValues = Bindings.Select(binding => binding() as IEnumerable).ToList();

                        if (bindingValues.Any(o => o == null))
                        {
                            throw new InvalidOperationException(
                                "Complex types cannot be binding sources for scalar values.");
                        }

                        if (bindingValues.Select(enumerable => enumerable.Cast<object>())
                            .Any(enumerable => !enumerable.SequenceEqual(enumerableValue.Cast<object>())))
                        {
                            throw new BindingException(
                                "Multiple bindings to a single source must have equivalent target values.");
                        }
                    }

                    value = CreateCollection(enumerableValue);
                }
                else
                {
                    value = Value;
                }

                return value;
            }
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var childSerializer = (ValueValueNode) CreateChildSerializer();
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var itemLength = GetConstFieldItemLength();

            var boundValue = BoundValue;

            var count = GetConstFieldCount();

            // handle null value case
            if (boundValue == null)
            {
                if (count != null)
                {
                    var defaultValue = TypeNode.GetDefaultValue(childSerializedType);
                    for (var i = 0; i < count.Value; i++)
                    {
                        childSerializer.Serialize(stream, defaultValue, childSerializedType, itemLength);
                    }
                }

                return;
            }

            PrimitiveCollectionSerializeOverride(stream, boundValue, childSerializer, childSerializedType, itemLength,
                count);

            SerializeTermination(stream, eventShuttle);
        }

        internal override async Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            var childSerializer = (ValueValueNode)CreateChildSerializer();
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var itemLength = GetConstFieldItemLength();

            var boundValue = BoundValue;

            var count = GetConstFieldCount();

            // handle null value case
            if (boundValue == null)
            {
                if (count != null)
                {
                    var defaultValue = TypeNode.GetDefaultValue(childSerializedType);
                    for (var i = 0; i < count.Value; i++)
                    {
                        await childSerializer
                            .SerializeAsync(stream, defaultValue, childSerializedType, itemLength, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }

                return;
            }

            PrimitiveCollectionSerializeOverride(stream, boundValue, childSerializer, childSerializedType, itemLength,
                count);

            SerializeTermination(stream, eventShuttle);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var items = DeserializeCollection(stream, eventShuttle).ToList();
            CreateFinalCollection(items);
        }

        internal override async Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            var items = await DeserializeCollectionAsync(stream, eventShuttle, cancellationToken).ConfigureAwait(false);
            CreateFinalCollection(items);
        }

        protected abstract void PrimitiveCollectionSerializeOverride(BoundedStream stream, object boundValue,
            ValueValueNode childSerializer, SerializedType childSerializedType, FieldLength itemLength, long? itemCount);

        protected abstract Task PrimitiveCollectionSerializeOverrideAsync(BoundedStream stream, object boundValue,
            ValueValueNode childSerializer, SerializedType childSerializedType, FieldLength itemLength, long? itemCount,
            CancellationToken cancellationToken);

        protected abstract object CreateCollection(long size);
        protected abstract object CreateCollection(IEnumerable enumerable);
        protected abstract void SetCollectionValue(object item, long index);

        protected override object GetLastItemValueOverride()
        {
            throw new InvalidOperationException(
                "Not supported on primitive collections.  Use SerializeUntil attribute.");
        }

        private void CreateFinalCollection(List<object> items)
        {
            var itemCount = items.Count;

            Value = CreateCollection(itemCount);

            /* Copy temp list into final collection */
            for (var i = 0; i < itemCount; i++)
            {
                SetCollectionValue(items[i], i);
            }
        }

        private IEnumerable<object> DeserializeCollection(BoundedStream stream, EventShuttle eventShuttle)
        {
            /* Create single serializer to do all the work */
            var childSerializer = (ValueValueNode) CreateChildSerializer();
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var terminationValue = GetTerminationValue();
            var terminationChild = GetTerminationChild();
            var itemLength = GetFieldItemLength();

            var reader = new BinaryReader(stream);
            var count = GetFieldCount() ?? long.MaxValue;

            for (long i = 0; i < count && !EndOfStream(stream); i++)
            {
                if (IsTerminated(stream, terminationChild, terminationValue, eventShuttle))
                {
                    break;
                }

                childSerializer.Deserialize(reader, childSerializedType, itemLength);
                yield return childSerializer.GetValue(childSerializedType);
            }
        }

        private async Task<List<object>> DeserializeCollectionAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            var list = new List<object>();

            /* Create single serializer to do all the work */
            var childSerializer = (ValueValueNode) CreateChildSerializer();
            var childSerializedType = childSerializer.TypeNode.GetSerializedType();

            var terminationValue = GetTerminationValue();
            var terminationChild = GetTerminationChild();
            var itemLength = GetFieldItemLength();

            var reader = new AsyncBinaryReader(stream);
            var count = GetFieldCount() ?? long.MaxValue;

            for (long i = 0; i < count && !EndOfStream(stream); i++)
            {
                if (IsTerminated(stream, terminationChild, terminationValue, eventShuttle))
                {
                    break;
                }

                await childSerializer.DeserializeAsync(reader, childSerializedType, itemLength, cancellationToken);
                var value = childSerializer.GetValue(childSerializedType);
                list.Add(value);
            }

            return list;
        }
    }
}