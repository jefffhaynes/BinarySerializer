using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        private object _cachedValue;
        private Type _valueType;

        public ObjectValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

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

                var objectTypeNode = (ObjectTypeNode) TypeNode;
                var subType = objectTypeNode.GetSubType(_valueType);

                var serializableChildren = GetSerializableChildren().ToList();

                if (subType.Constructor == null)
                    throw new InvalidOperationException("No public constructors.");

                object value;

                if (subType.ConstructorParameterNames.Length == 0)
                {
                    value = subType.Constructor.Invoke(null);
                }
                else
                {
                    var parameterizedChildren = subType.ConstructorParameterNames.Join(serializableChildren,
                        parameter => parameter.ToLower(), serializableChild => serializableChild.Name.ToLower(),
                        (parameter, serializableChild) => serializableChild).ToList();

                    var parameterValues = parameterizedChildren.Select(child => child.Value).ToArray();

                    value = subType.Constructor.Invoke(parameterValues);

                    serializableChildren = serializableChildren.Except(parameterizedChildren).ToList();
                }

                foreach (var child in serializableChildren)
                {
                    var setter = child.TypeNode.ValueSetter;

                    if(setter == null)
                        throw new InvalidOperationException("No public setter.");

                    setter(value, child.Value);
                }

                return value;
            }

            set
            {
                if (Children.Count > 0)
                    throw new InvalidOperationException("Value already set.");

                if (value == null)
                    return;

                var typeNode = (ObjectTypeNode) TypeNode;

                var valueType = value.GetType();

                var subType = typeNode.GetSubType(valueType);

                Children = new List<ValueNode>(subType.Children.Select(child => child.CreateSerializer(this)));

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

            var serializationContextLazy = new Lazy<BinarySerializationContext>(CreateSerializationContext);

            foreach (var child in serializableChildren)
            {
                if (eventShuttle != null && eventShuttle.HasSerializationSubscribers)
                    eventShuttle.OnMemberSerializing(this, child.Name, serializationContextLazy.Value,
                        stream.GlobalRelativePosition);

                child.Serialize(stream, eventShuttle);

                if (eventShuttle != null && eventShuttle.HasSerializationSubscribers)
                    eventShuttle.OnMemberSerialized(this, child.Name, child.BoundValue, serializationContextLazy.Value,
                        stream.GlobalRelativePosition);
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

            if (_valueType != null)
            {
                var typeNode = (ObjectTypeNode) TypeNode;

                var subType = typeNode.GetSubType(_valueType);
                Children = new List<ValueNode>(subType.Children.Select(child => child.CreateSerializer(this)));

                var serializationContextLazy = new Lazy<BinarySerializationContext>(CreateSerializationContext);

                foreach (var child in GetSerializableChildren())
                {
                    if (eventShuttle != null && eventShuttle.HasDeserializationSubscribers)
                        eventShuttle.OnMemberDeserializing(this, child.Name, serializationContextLazy.Value,
                            stream.GlobalRelativePosition);

                    if (ShouldTerminate(stream))
                        break;

                    child.Deserialize(stream, eventShuttle);

                    if (eventShuttle != null && eventShuttle.HasDeserializationSubscribers)
                        eventShuttle.OnMemberDeserialized(this, child.Name, child.Value, serializationContextLazy.Value,
                            stream.GlobalRelativePosition);
                }
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
    }
}