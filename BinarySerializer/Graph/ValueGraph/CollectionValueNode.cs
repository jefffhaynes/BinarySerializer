using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class CollectionValueNode : CollectionValueNodeBase
    {
        protected CollectionValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializableChildren = GetSerializableChildren().ToList();

            SetTerminationValue(serializableChildren);

            foreach (var child in serializableChildren)
            {
                if (stream.IsAtLimit)
                {
                    break;
                }

                var childStream = new BoundedStream(stream, GetConstFieldItemLength);

                child.Serialize(childStream, eventShuttle);
            }

            SerializeTermination(stream, eventShuttle);
        }

        internal override async Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            var serializableChildren = GetSerializableChildren().ToList();

            SetTerminationValue(serializableChildren);

            foreach (var child in serializableChildren)
            {
                if (stream.IsAtLimit)
                {
                    break;
                }

                var childStream = new BoundedStream(stream, GetConstFieldItemLength);

                await child.SerializeAsync(childStream, eventShuttle, true, cancellationToken).ConfigureAwait(false);
            }

            await SerializeTerminationAsync(stream, eventShuttle, cancellationToken);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var terminationValue = GetTerminationValue();
            var terminationChild = GetTerminationChild();
            var itemTerminationValue = GetItemTerminationValue();
            var itemLengths = GetItemLengths();

            using (var itemLengthEnumerator = itemLengths?.GetEnumerator())
            {
                var count = GetFieldCount() ?? long.MaxValue;

                for (long i = 0; i < count && !EndOfStream(stream); i++)
                {
                    if (IsTerminated(stream, terminationChild, terminationValue, eventShuttle))
                    {
                        break;
                    }

                    itemLengthEnumerator?.MoveNext();

                    // TODO this doesn't allow for deferred eval of endianness in the case of jagged arrays
                    // probably extremely rare but still...
                    var itemLength = itemLengthEnumerator?.Current;
                    var childStream = itemLength == null
                        ? new BoundedStream(stream)
                        : new BoundedStream(stream, () => itemLength);

                    var child = CreateChildSerializer();

                    using (var streamResetter = new StreamResetter(childStream))
                    {
                        child.Deserialize(childStream, eventShuttle);

                        if (IsTerminated(child, itemTerminationValue))
                        {
                            ProcessLastItem(streamResetter, child);
                            break;
                        }

                        streamResetter.CancelReset();
                    }

                    Children.Add(child);
                }
            }
        }

        internal override async Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            var terminationValue = GetTerminationValue();
            var terminationChild = GetTerminationChild();
            var itemTerminationValue = GetItemTerminationValue();
            var itemLengths = GetItemLengths();

            using (var itemLengthEnumerator = itemLengths?.GetEnumerator())
            {
                var count = GetFieldCount() ?? long.MaxValue;

                for (long i = 0; i < count && !EndOfStream(stream); i++)
                {
                    if (IsTerminated(stream, terminationChild, terminationValue, eventShuttle))
                    {
                        break;
                    }

                    itemLengthEnumerator?.MoveNext();

                    // TODO this doesn't allow for deferred eval of endianness in the case of jagged arrays
                    // probably extremely rare but still...
                    var itemLength = itemLengthEnumerator?.Current;
                    var childStream = itemLength == null
                        ? new BoundedStream(stream)
                        : new BoundedStream(stream, () => itemLength);

                    var child = CreateChildSerializer();

                    using (var streamResetter = new StreamResetter(childStream))
                    {
                        await child.DeserializeAsync(childStream, eventShuttle, cancellationToken)
                            .ConfigureAwait(false);

                        if (IsTerminated(child, itemTerminationValue))
                        {
                            ProcessLastItem(streamResetter, child);
                            break;
                        }

                        streamResetter.CancelReset();
                    }

                    Children.Add(child);
                }
            }
        }

        protected override long CountOverride()
        {
            return Children.Count;
        }

        protected override IEnumerable<long> MeasureItemsOverride()
        {
            var nullStream = new NullStream();
            var boundedStream = new BoundedStream(nullStream);

            var serializableChildren = GetSerializableChildren();

            return serializableChildren.Select(child =>
            {
                boundedStream.RelativePosition = 0;
                child.Serialize(boundedStream, null);
                return boundedStream.RelativePosition;
            });
        }

        protected override object GetLastItemValueOverride()
        {
            var lastItem = Children.LastOrDefault();
            if (lastItem == null)
            {
                throw new InvalidOperationException("Unable to determine last item value because collection is empty.");
            }

            var terminationItemChild =
                (ValueValueNode) lastItem.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);
            return terminationItemChild.BoundValue;
        }

        private void SetTerminationValue(List<ValueNode> serializableChildren)
        {
            var typeNode = (CollectionTypeNode) TypeNode;

            if (typeNode.ItemSerializeUntilAttribute == null ||
                typeNode.ItemSerializeUntilAttribute.LastItemMode != LastItemMode.Include)
            {
                return;
            }

            var lastChild = serializableChildren.LastOrDefault();

            if (lastChild == null)
            {
                return;
            }

            var itemTerminationValue = TypeNode.ItemSerializeUntilBinding.GetBoundValue(this);
            var itemTerminationChild = lastChild.GetChild(typeNode.ItemSerializeUntilAttribute.ItemValuePath);

            var convertedItemTerminationValue =
                itemTerminationValue.ConvertTo(itemTerminationChild.TypeNode.Type);

            itemTerminationChild.Value = convertedItemTerminationValue;
        }

        private object GetItemTerminationValue()
        {
            object itemTerminationValue = null;
            if (TypeNode.ItemSerializeUntilBinding != null)
            {
                itemTerminationValue = TypeNode.ItemSerializeUntilBinding.GetValue(this);
            }
            return itemTerminationValue;
        }

        private void ProcessLastItem(StreamResetter streamResetter, ValueNode child)
        {
            switch (TypeNode.ItemSerializeUntilAttribute.LastItemMode)
            {
                case LastItemMode.Include:
                    streamResetter.CancelReset();
                    Children.Add(child);
                    break;
                case LastItemMode.Discard:
                    streamResetter.CancelReset();
                    break;
                case LastItemMode.Defer:
                    // stream will reset
                    break;
            }
        }

        private IEnumerable<long> GetItemLengths()
        {
            IEnumerable<long> itemLengths = null;
            if (TypeNode.ItemLengthBindings != null)
            {
                var itemLengthValue = TypeNode.ItemLengthBindings.GetValue(this);

                var enumerableItemLengthValue = itemLengthValue as IEnumerable;

                itemLengths = enumerableItemLengthValue?.Cast<object>().Select(Convert.ToInt64) ??
                              GetInfiniteSequence(Convert.ToInt64(itemLengthValue));
            }
            return itemLengths;
        }

        private bool IsTerminated(ValueNode child, object itemTerminationValue)
        {
            if (TypeNode.ItemSerializeUntilBinding == null)
            {
                return false;
            }

            var itemTerminationChild = child.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);

            var convertedItemTerminationValue =
                itemTerminationValue.ConvertTo(itemTerminationChild.TypeNode.Type);

            return itemTerminationChild.Value == null ||
                   itemTerminationChild.Value.Equals(convertedItemTerminationValue);
        }

        // ReSharper disable IteratorNeverReturns
        private static IEnumerable<long> GetInfiniteSequence(long value)
        {
            while (true)
            {
                yield return value;
            }
        }
        // ReSharper restore IteratorNeverReturns
    }
}