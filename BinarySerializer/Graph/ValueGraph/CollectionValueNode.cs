using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class CollectionValueNode : ValueNode
    {
        protected CollectionValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void SerializeOverride(Stream stream, EventShuttle eventShuttle)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
            {
                child.Serialize(stream, eventShuttle);
            }

            var typeNode = (CollectionTypeNode)TypeNode;

            if (typeNode.TerminationChild != null)
            {
                var terminationChild = typeNode.TerminationChild.CreateSerializer(this);
                terminationChild.Value = typeNode.TerminationValue;
                terminationChild.Serialize(stream, eventShuttle);
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            var count = TypeNode.FieldCountBinding != null ? Convert.ToInt32(TypeNode.FieldCountBinding.GetValue(this)) : int.MaxValue;

            var terminationValue = typeNode.TerminationValue;
            var terminationChild = typeNode.TerminationChild == null ? null : typeNode.TerminationChild.CreateSerializer(this);

            for (int i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                /* Check termination case */
                if (terminationChild != null)
                {
                    using (var streamResetter = new StreamResetter(stream))
                    {
                        terminationChild.Deserialize(stream);

                        if (terminationChild.Value.Equals(terminationValue))
                        {
                            streamResetter.CancelReset();
                            break;
                        }
                    }
                }

                var child = typeNode.Child.CreateSerializer(this);
                child.Deserialize(stream);

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

        protected override long CountOverride()
        {
            return Children.Count();
        }

        protected override long MeasureItemOverride()
        {
            var nullStream = new NullStream();
            var streamKeeper = new StreamKeeper(nullStream);

            var serializableChildren = GetSerializableChildren();

            var childLengths = serializableChildren.Select(child =>
            {
                streamKeeper.RelativePosition = 0;
                child.Serialize(streamKeeper, null);
                return streamKeeper.RelativePosition;
            }).ToList();

            if (!childLengths.Any())
                return 0;

            var childLengthGroups = childLengths.GroupBy(childLength => childLength);

            var childLengthGroup = childLengthGroups.SingleOrDefault();

            if (childLengthGroup == null)
                throw new InvalidOperationException("Unable to update binding source because not all items have equal lengths.");

            return childLengthGroup.Key;
        }

        protected override object GetLastItemValueOverride()
        {
            var lastItem = (ValueNode)Children.LastOrDefault();
            if(lastItem == null)
                throw new InvalidOperationException("Can't determine last item value because collection is empty.");

            var terminationItemChild = (ValueValueNode)lastItem.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);
            return terminationItemChild.BoundValue;
        }
    }
}
