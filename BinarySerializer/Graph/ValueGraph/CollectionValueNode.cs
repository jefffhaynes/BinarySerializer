using System;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class CollectionValueNode : ValueNode
    {
        protected CollectionValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected override void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializableChildren = GetSerializableChildren();

            int? itemLength = null;
            if (TypeNode.ItemLengthBinding != null && TypeNode.ItemLengthBinding.IsConst)
                itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.ConstValue);

            foreach (var child in serializableChildren)
            {
                if (stream.IsAtLimit)
                    break;

                var childStream = itemLength == null ? stream : new StreamLimiter(stream, itemLength.Value);
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

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            var count = TypeNode.FieldCountBinding != null ? Convert.ToInt32(TypeNode.FieldCountBinding.GetValue(this)) : int.MaxValue;

            var terminationValue = typeNode.TerminationValue;
            var terminationChild = typeNode.TerminationChild == null ? null : typeNode.TerminationChild.CreateSerializer(this);

            int? itemLength = null;
            if (TypeNode.ItemLengthBinding != null)
                itemLength = Convert.ToInt32(TypeNode.ItemLengthBinding.GetValue(this));

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

                var child = typeNode.Child.CreateSerializer(this);

                var childStream = itemLength == null ? stream : new StreamLimiter(stream, itemLength.Value);
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

        protected override long CountOverride()
        {
            return Children.Count();
        }

        protected override long MeasureItemOverride()
        {
            var nullStream = new NullStream();
            var streamLimiter = new StreamLimiter(nullStream);

            var serializableChildren = GetSerializableChildren();

            var childLengths = serializableChildren.Select(child =>
            {
                streamLimiter.RelativePosition = 0;
                child.Serialize(streamLimiter, null);
                return streamLimiter.RelativePosition;
            }).ToList();

            if (!childLengths.Any())
                return 0;

            var childLengthGroups = childLengths.GroupBy(childLength => childLength).ToList();

            if (childLengthGroups.Count > 1)
                throw new InvalidOperationException("Unable to update binding source because not all items have equal lengths.");

            var childLengthGroup = childLengthGroups.Single();

            return childLengthGroup.Key;
        }

        protected override object GetLastItemValueOverride()
        {
            var lastItem = Children.LastOrDefault();
            if(lastItem == null)
                throw new InvalidOperationException("Can't determine last item value because collection is empty.");

            var terminationItemChild = (ValueValueNode)lastItem.GetChild(TypeNode.ItemSerializeUntilAttribute.ItemValuePath);
            return terminationItemChild.BoundValue;
        }
    }
}
