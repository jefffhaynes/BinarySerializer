﻿using System;
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
            var serializableChildren = GetSerializableChildren();

            int? itemLength = null;
            if (TypeNode.ItemLengthBinding != null && TypeNode.ItemLengthBinding.IsConst)
                itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.ConstValue);

            foreach (var child in serializableChildren)
            {
                if (stream.IsAtLimit)
                    break;

                var childStream = itemLength == null ? stream : new BoundedStream(stream, itemLength.Value);
                child.Serialize(childStream, eventShuttle);
            }

            var typeNode = (CollectionTypeNode)TypeNode;

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

            var count = TypeNode.FieldCountBinding != null ? Convert.ToInt32(TypeNode.FieldCountBinding.GetValue(this)) : Int32.MaxValue;

            var terminationValue = typeNode.TerminationValue;
            var terminationChild = typeNode.TerminationChild?.CreateSerializer(this);

            IEnumerable<int> itemLengths = null;
            if (TypeNode.ItemLengthBinding != null)
            {
                var itemLengthValue = TypeNode.ItemLengthBinding.GetValue(this);

                var enumerableItemLengthValue = itemLengthValue as IEnumerable;

                itemLengths = enumerableItemLengthValue?.Cast<object>().Select(Convert.ToInt32) ?? GetInfiniteSequence(Convert.ToInt32(itemLengthValue));
            }
                      
            IEnumerator<int> itemLengthEnumerator = null;

            try
            {
                if (itemLengths != null)
                    itemLengthEnumerator = itemLengths.GetEnumerator();

                for (int i = 0; i < count; i++)
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

                    var childStream = itemLengthEnumerator == null
                        ? stream
                        : new BoundedStream(stream, itemLengthEnumerator.Current);

                    child.Deserialize(childStream, eventShuttle);

                    /* Check child termination case */
                    if (TypeNode.ItemSerializeUntilBinding != null)
                    {
                        var itemTerminationValue = TypeNode.ItemSerializeUntilBinding.GetValue(this);
                        var itemTerminationChild = child.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);

                        if (itemTerminationChild.Value.Equals(itemTerminationValue))
                        {
                            if (!TypeNode.ItemSerializeUntilAttribute.ExcludeLastItem)
                            {
                                Children.Add(child);
                            }
                            break;
                        }
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
        
        private static IEnumerable<int> GetInfiniteSequence(int value)
        {
            while (true)
                yield return value;
            // ReSharper disable FunctionNeverReturns
        }
        // ReSharper restore FunctionNeverReturns
    }
}
