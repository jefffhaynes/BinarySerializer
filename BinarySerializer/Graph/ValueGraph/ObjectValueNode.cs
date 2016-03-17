using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

#if WINDOWS_UWP
using System.Reflection;
#endif

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        private object _cachedValue;
        private Type _valueType;

#if WINDOWS_UWP
        private TypeInfo _valueTypeInfo;
#endif

        public ObjectValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

        public override object BoundValue
        {
            get { return GetValue(child => child.BoundValue); }
        }

        public override object Value
        {
            get
            {
                /* For creating serialization contexts quickly */
                if (_cachedValue != null)
                    return _cachedValue;

                return GetValue(child => child.Value);
            }

            set
            {
                if (value == null)
                    return;

                var typeNode = (ObjectTypeNode) TypeNode;

                var valueType = value.GetType();

                // always check for user-defined subtypes, whether they exist or not. 
                // In the trivial case we get the value type node back.
                var subType = typeNode.GetSubType(valueType);

                // create all child serializers
                Children = subType.Children.Select(child => child.CreateSerializer(this)).ToList();

                // initialize all children from the corresponding set value fields
                foreach (var child in Children)
                    child.Value = child.TypeNode.ValueGetter(value);

                _valueType = value.GetType();

#if WINDOWS_UWP
                _valueTypeInfo = _valueType.GetTypeInfo();
#endif

                // cache the value for creating serialization contexts quickly
                _cachedValue = value;
            }
        }

        private object GetValue(Func<ValueNode, object> childValueSelector)
        {
            if (_valueType == null)
                return null;

#if WINDOWS_UWP
            if(_valueTypeInfo.IsAbstract)
#else
            if (_valueType.IsAbstract)
#endif
                return null;

            var objectTypeNode = (ObjectTypeNode)TypeNode;
            var subType = objectTypeNode.GetSubType(_valueType);

            if (subType.Constructor == null)
                throw new InvalidOperationException("No public constructors.");

            IEnumerable<ValueNode> nonparameterizedChildren = Children.ToList();

            object value;

            if (subType.ConstructorParameterNames.Length == 0)
            {
                // handle simple case of no parameterized constructors
                value = subType.CompiledConstructor();
            }
            else
            {
                // find best match for constructor
                var serializableChildren = GetSerializableChildren();

                // find children that can be initialized or partially initialized with construction
                var parameterizedChildren = subType.ConstructorParameterNames.Join(serializableChildren,
                    parameter => parameter.ToLower(), 
                    serializableChild => serializableChild.Name.ToLower(),
                    (parameter, serializableChild) => serializableChild).ToList();

                var parameterValues = parameterizedChildren.Select(childValueSelector).ToArray();

                value = subType.Constructor.Invoke(parameterValues);
                
                nonparameterizedChildren = Children.Except(parameterizedChildren).ToList();
            }

            foreach (var child in nonparameterizedChildren)
            {
                var setter = child.TypeNode.ValueSetter;

                if (setter == null && !child.TypeNode.IsIgnored)
                    throw new InvalidOperationException("No public setter.");

                setter?.Invoke(value, childValueSelector(child));
            }

            return value;
        }

        internal override void SerializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            ObjectSerializeOverride(stream, eventShuttle);

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

        protected virtual void ObjectSerializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            // check to see if we are actually supposed to be a custom serialization.  This is a side-effect of
            // treating all object members as object nodes.  In the case of sub-types we could later discover we
            // are actually a custom node because the specified subtype implements IBinarySerializable.
            if (_valueType != null && TypeNode.SubtypeBinding != null)
            {
                var typeNode = (ObjectTypeNode)TypeNode;
                var subType = typeNode.GetSubType(_valueType);

                if (subType is CustomTypeNode)
                {
                    var customValueNode = subType.CreateSerializer((ValueNode)Parent);

                    // this is a little bit of a cheat, but another side-effect of this weird corner case
                    customValueNode.Value = _cachedValue;
                    customValueNode.SerializeOverride(stream, eventShuttle);
                    return;
                }
            }

            var serializableChildren = GetSerializableChildren();

            var serializationContextLazy = new Lazy<BinarySerializationContext>(CreateSerializationContext);
            
            foreach (var child in serializableChildren)
            {
                // report on serialization start if subscribed
                if (eventShuttle != null && eventShuttle.HasSerializationSubscribers)
                    eventShuttle.OnMemberSerializing(this, child.Name, serializationContextLazy.Value,
                        stream.GlobalRelativePosition);

                // serialize child
                child.Serialize(stream, eventShuttle);

                // report on serialization complete if subscribed
                if (eventShuttle != null && eventShuttle.HasSerializationSubscribers)
                    eventShuttle.OnMemberSerialized(this, child.Name, child.BoundValue, serializationContextLazy.Value,
                        stream.GlobalRelativePosition);
            }
        }

        internal override void DeserializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            // resolve value type for deserialization
            if (TypeNode.SubtypeBinding == null)
            {
                // trivial case with no subtypes
                _valueType = TypeNode.Type;

#if WINDOWS_UWP
                _valueTypeInfo = _valueType.GetTypeInfo();

                if (_valueTypeInfo.IsAbstract)
#else

                if (_valueType.IsAbstract)
#endif
                    throw new InvalidOperationException("Abstract types must have at least one subtype binding to be deserialized.");

            }
            else
            {
                // try to resolve value type using subtype mapping
                var subTypeValue = TypeNode.SubtypeBinding.GetValue(this);

                var matchingAttribute =
                    TypeNode.SubtypeAttributes.SingleOrDefault(
                        attribute =>
                            subTypeValue.Equals(Convert.ChangeType(attribute.Value, subTypeValue.GetType(), null)));

                _valueType = matchingAttribute?.Subtype;
            }

            // skip over if null (this may happen if subtypes are unknown during deserialization)
            if (_valueType != null)
            {
                var typeNode = (ObjectTypeNode) TypeNode;

                // generate correct children for this subtype
                var subType = typeNode.GetSubType(_valueType);
                Children = new List<ValueNode>(subType.Children.Select(child => child.CreateSerializer(this)));

                ObjectDeserializeOverride(stream, eventShuttle);
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

        protected virtual void ObjectDeserializeOverride(LimitedStream stream, EventShuttle eventShuttle)
        {
            // check to see if we are actually supposed to be a custom deserialization.  This is a side-effect of
            // treating all object members as object nodes.  In the case of sub-types we could later discover we
            // are actually a custom node because the specified subtype implements IBinarySerializable.
            if (_valueType != null && TypeNode.SubtypeBinding != null)
            {
                var typeNode = (ObjectTypeNode)TypeNode;
                var subType = typeNode.GetSubType(_valueType);

                if (subType is CustomTypeNode)
                {
                    var customValueNode = subType.CreateSerializer((ValueNode)Parent);
                    customValueNode.DeserializeOverride(stream, eventShuttle);

                    // this is a cheat, but another side-effect of this weird corner case
                    _cachedValue = customValueNode.Value;
                    return;
                }
            }

            var serializationContextLazy = new Lazy<BinarySerializationContext>(CreateSerializationContext);

            foreach (var child in GetSerializableChildren())
            {
                // report on deserialization start if subscribed
                if (eventShuttle != null && eventShuttle.HasDeserializationSubscribers)
                    eventShuttle.OnMemberDeserializing(this, child.Name, serializationContextLazy.Value,
                        stream.GlobalRelativePosition);

                // deserialize child
                child.Deserialize(stream, eventShuttle);

                // report on deserialization complete if subscribed
                if (eventShuttle != null && eventShuttle.HasDeserializationSubscribers)
                    eventShuttle.OnMemberDeserialized(this, child.Name, child.Value, serializationContextLazy.Value,
                        stream.GlobalRelativePosition);
            }
        }

        protected override Type GetValueTypeOverride()
        {
            return _valueType;
        }
    }
}