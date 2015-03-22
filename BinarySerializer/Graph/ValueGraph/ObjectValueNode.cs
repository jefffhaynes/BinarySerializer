using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        public ObjectValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

        private Type _valueType;

        private object _cachedValue;

        public override object Value
        {
            get
            {
                if (_valueType == null)
                    return null;

                /* For creating serialization contexts quickly */
                if (_cachedValue != null)
                    return _cachedValue;

                if (_valueType.IsAbstract)
                    return null;

                var value = Activator.CreateInstance(_valueType);

                var serializableChildren = GetSerializableChildren();

                foreach (var child in serializableChildren)
                    child.TypeNode.ValueSetter(value, child.Value);

                return value;
            }

            set
            {
                if (Children.Any())
                    throw new InvalidOperationException("Value already set.");

                if (value == null)
                    return;

                var typeNode = (ObjectTypeNode) TypeNode;

                var valueType = value.GetType();

                IEnumerable<TypeNode> typeChildren = typeNode.GetTypeChildren(valueType);

                Children = new List<ValueNode>(typeChildren.Select(child => child.CreateSerializer(this)));

                var serializableChildren = GetSerializableChildren();

                foreach (var child in serializableChildren)
                    child.Value = child.TypeNode.ValueGetter(value);

                _valueType = value.GetType();

                /* For creating serialization contexts quickly */
                _cachedValue = value;
            }
        }

        protected override void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            var serializableChildren = GetSerializableChildren();

            var serializationContext = CreateSerializationContext();

            foreach (var child in serializableChildren)
            {
                if (eventShuttle != null)
                    eventShuttle.OnMemberSerializing(this, child.Name, serializationContext, GetOffset(stream));

                child.Serialize(stream, eventShuttle);

                if (eventShuttle != null)
                    eventShuttle.OnMemberSerialized(this, child.Name, child.BoundValue, serializationContext, GetOffset(stream));
            }

            /* Check if we need to pad out object */
            if (TypeNode.FieldLengthBinding != null)
            {
                var length = Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this));

                if (length > stream.RelativePosition)
                {
                    var padLength = length - stream.RelativePosition;
                    var pad = new byte[padLength];
                    stream.Write(pad, 0, pad.Length);
                }
            }
        }

        public override void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle)
        {
            if (TypeNode.SubtypeBinding == null)
            {
                _valueType = TypeNode.Type;
            }
            else
            {
                var subTypeValue = TypeNode.SubtypeBinding.GetValue(this);

                var matchingAttribute =
                    TypeNode.SubtypeAttributes.SingleOrDefault(
                        attribute =>
                            subTypeValue.Equals(Convert.ChangeType(attribute.Value, subTypeValue.GetType(), null)));

                _valueType = matchingAttribute == null ? null : matchingAttribute.Subtype;
            }

            if (_valueType == null)
                return;

            var typeNode = (ObjectTypeNode)TypeNode;

            var typeChildren = typeNode.GetTypeChildren(_valueType);
            Children = new List<ValueNode>(typeChildren.Select(child => child.CreateSerializer(this)));

            var context = CreateSerializationContext();

            foreach (var child in GetSerializableChildren())
            {
                if(eventShuttle != null)
                    eventShuttle.OnMemberDeserializing(this, child.Name, context, GetOffset(stream));

                if (ShouldTerminate(stream))
                    break;

                child.Deserialize(stream, eventShuttle);

                if(eventShuttle != null)
                    eventShuttle.OnMemberDeserialized(this, child.Name, child.Value, context, GetOffset(stream));
            }

            /* Check if we need to read past padding */
            if (TypeNode.FieldLengthBinding != null)
            {
                var length = Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this));

                if (length > stream.RelativePosition)
                {
                    var padLength = length - stream.RelativePosition;
                    var pad = new byte[padLength];
                    stream.Read(pad, 0, pad.Length);
                }
            }
        }

        protected override Type GetValueTypeOverride()
        {
            return _valueType;
        }

        private static long GetOffset(StreamLimiter streamLimiter)
        {
            var ancestors = GetStreamLimiterAncestry(streamLimiter);
            return ancestors.Sum(limiter => limiter.RelativePosition);
        }

        private static IEnumerable<StreamLimiter> GetStreamLimiterAncestry(StreamLimiter decendant)
        {
            var parent = decendant;

            while (parent != null)
            {
                yield return parent;
                parent = parent.Source as StreamLimiter;
            }
        }
    }
}
