using System;
using System.Collections;
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

        private IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Cast<ValueNode>().Where(child => child.TypeNode.IgnoreAttribute == null);
        }

        protected override void SerializeOverride(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
            {
                child.Serialize(stream);
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            var count = TypeNode.FieldCountBinding != null ? Convert.ToInt32(TypeNode.FieldCountBinding.GetValue(this)) : int.MaxValue;

            int? length = null;
            if (TypeNode.ItemLengthBinding != null)
                length = Convert.ToInt32(TypeNode.ItemLengthBinding.GetValue(this));

            for (int i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                var child = (ValueValueNode)typeNode.Child.CreateSerializer(this);
                child.Value = child.Deserialize(stream, child.TypeNode.GetSerializedType(), length);
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
                child.Serialize(streamKeeper);
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
    }
}
