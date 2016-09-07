using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class CollectionValueNode : ValueNode
    {
        protected CollectionValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var serializableChildren = GetSerializableChildren().ToList();

            var typeNode = (CollectionTypeNode)TypeNode;

            if (typeNode.ItemSerializeUntilAttribute != null &&
                typeNode.ItemSerializeUntilAttribute.LastItemMode == LastItemMode.Include)
            {
                var lastChild = serializableChildren.LastOrDefault();

                if (lastChild != null)
                {
                    var itemTerminationValue = TypeNode.ItemSerializeUntilBinding.GetBoundValue(this);
                    var itemTerminationChild = lastChild.GetChild(typeNode.ItemSerializeUntilAttribute.ItemValuePath);

                    var convertedItemTerminationValue =
                        itemTerminationValue.ConvertTo(itemTerminationChild.TypeNode.Type);

                    itemTerminationChild.Value = convertedItemTerminationValue;
                }
            }
            
            foreach (var child in serializableChildren)
            {
                if (stream.IsAtLimit)
                    break;

                var childStream = GetConstFieldItemLength() == null
                    ? stream
                    : new BoundedStream(stream, GetConstFieldItemLength);

                child.Serialize(childStream, eventShuttle);
            }

            if (typeNode.TerminationChild != null)
            {
                var terminationChild = typeNode.TerminationChild.CreateSerializer(this);
                terminationChild.Value = typeNode.TerminationValue;
                terminationChild.Serialize(stream, eventShuttle);
            }
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            var count = GetFieldCount() ?? long.MaxValue;

            var terminationValue = typeNode.TerminationValue;
            var terminationChild = typeNode.TerminationChild?.CreateSerializer(this);

            object itemTerminationValue = null;
            if (TypeNode.ItemSerializeUntilBinding != null)
            {
                itemTerminationValue = TypeNode.ItemSerializeUntilBinding.GetValue(this);
            }

            IEnumerable<long> itemLengths = null;
            if (TypeNode.ItemLengthBindings != null)
            {
                var itemLengthValue = TypeNode.ItemLengthBindings.GetValue(this);

                var enumerableItemLengthValue = itemLengthValue as IEnumerable;

                itemLengths = enumerableItemLengthValue?.Cast<object>().Select(Convert.ToInt64) ?? 
                    GetInfiniteSequence(Convert.ToInt64(itemLengthValue));
            }
                      
            IEnumerator<long> itemLengthEnumerator = null;

            try
            {
                if (itemLengths != null)
                    itemLengthEnumerator = itemLengths.GetEnumerator();

                for (long i = 0; i < count; i++)
                {
                    if (EndOfStream(stream))
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

                    var child = typeNode.Child.CreateSerializer(this);

                    itemLengthEnumerator?.MoveNext();

                    // TODO this doesn't allow for deferred eval of endianness in the case of jagged arrays
                    // probably extremely rare but still...
                    var itemLength = itemLengthEnumerator?.Current;
                    var childStream = itemLength == null ? stream : new BoundedStream(stream, () => itemLength);
                    
                    using (var streamResetter = new StreamResetter(childStream))
                    {
                        child.Deserialize(childStream, eventShuttle);

                        /* Check child termination case */
                        if (TypeNode.ItemSerializeUntilBinding != null)
                        {
                            var itemTerminationChild = child.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);

                            var convertedItemTerminationValue =
                                itemTerminationValue.ConvertTo(itemTerminationChild.TypeNode.Type);

                            if (itemTerminationChild.Value.Equals(convertedItemTerminationValue))
                            {
#pragma warning disable 618
                                if (TypeNode.ItemSerializeUntilAttribute.ExcludeLastItem)
#pragma warning restore 618
                                {
                                    streamResetter.CancelReset();
                                    break;
                                }

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

                                break;
                            }
                        }

                        streamResetter.CancelReset();
                    }

                    Children.Add(child);
                }
            }
            finally
            {
                itemLengthEnumerator?.Dispose();
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
            if(lastItem == null)
                throw new InvalidOperationException("Unable to determine last item value because collection is empty.");

            var terminationItemChild = (ValueValueNode)lastItem.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);
            return terminationItemChild.BoundValue;
        }
        
        private static IEnumerable<long> GetInfiniteSequence(long value)
        {
            while (true)
                yield return value;
            // ReSharper disable FunctionNeverReturns
        }
        // ReSharper restore FunctionNeverReturns
    }
}
