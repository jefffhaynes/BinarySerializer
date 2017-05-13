using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
                {
                    return _cachedValue;
                }

                return GetValue(child => child.Value);
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                var typeNode = (ObjectTypeNode) TypeNode;

                var valueType = value.GetType();

                // always check for user-defined subtypes, whether they exist or not. 
                // In the trivial case we get the value type node back.
                var subType = typeNode.GetSubTypeNode(valueType);

                // create all child serializers
                Children = subType.Children.Select(child => child.CreateSerializer(this)).ToList();

                // initialize all children from the corresponding set value fields
                foreach (var child in Children)
                {
                    child.Value = child.TypeNode.ValueGetter(value);
                }

                _valueType = value.GetType();

                // cache the value for creating serialization contexts quickly
                _cachedValue = value;
            }
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            ObjectSerializeOverride(stream, eventShuttle);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            ResolveValueType();

            // skip over if null (this may happen if subtypes are unknown during deserialization)
            if (_valueType != null)
            {
                GenerateChildren();

                try
                {
                    ObjectDeserializeOverride(stream, eventShuttle);
                }
                catch (EndOfStreamException)
                {
                    // this is ok but we can't consider this object fully formed.
                    _valueType = null;
                }
            }

            SkipPadding(stream);
        }

        internal override async Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            ResolveValueType();

            // skip over if null (this may happen if subtypes are unknown during deserialization)
            if (_valueType != null)
            {
                GenerateChildren();

                try
                {
                    await ObjectDeserializeOverrideAsync(stream, eventShuttle, cancellationToken).ConfigureAwait(false);
                }
                catch (EndOfStreamException)
                {
                    // this is ok but we can't consider this object fully formed.
                    _valueType = null;
                }
            }

            SkipPadding(stream);
        }

        protected virtual void ObjectSerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            // check to see if we are actually supposed to be a custom serialization.  This is a side-effect of
            // treating all object members as object nodes.  In the case of sub-types we could later discover we
            // are actually a custom node because the specified subtype implements IBinarySerializable.

            if (IsCustomNode(out ValueNode customValueNode))
            {
                // this is a little bit of a cheat, but another side-effect of this weird corner case
                customValueNode.Value = _cachedValue;

                customValueNode.SerializeOverride(stream, eventShuttle);

                return;
            }

            var serializableChildren = GetSerializableChildren();

            var lazyContext = CreateLazySerializationContext();

            foreach (var child in serializableChildren)
            {
                EmitBeginSerialization(stream, child, lazyContext, eventShuttle);

                child.Serialize(stream, eventShuttle);

                EmitEndSerialization(stream, child, lazyContext, eventShuttle);
            }
        }

        protected virtual void ObjectDeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            // check to see if we are actually supposed to be a custom deserialization.  This is a side-effect of
            // treating all object members as object nodes.  In the case of sub-types we could later discover we
            // are actually a custom node because the specified subtype implements IBinarySerializable.
            if (IsCustomNode(out ValueNode customValueNode))
            {
                customValueNode.DeserializeOverride(stream, eventShuttle);

                // this is a cheat, but another side-effect of this weird corner case
                _cachedValue = customValueNode.Value;

                return;
            }

            var lazyContext = CreateLazySerializationContext();

            foreach (var child in GetSerializableChildren())
            {
                EmitBeginDeserialization(stream, child, lazyContext, eventShuttle);

                child.Deserialize(stream, eventShuttle);

                EmitEndDeserialization(stream, child, lazyContext, eventShuttle);
            }
        }

        protected virtual async Task ObjectDeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            // check to see if we are actually supposed to be a custom deserialization.  This is a side-effect of
            // treating all object members as object nodes.  In the case of sub-types we could later discover we
            // are actually a custom node because the specified subtype implements IBinarySerializable.
            if (IsCustomNode(out ValueNode customValueNode))
            {
                await customValueNode.DeserializeOverrideAsync(stream, eventShuttle, cancellationToken)
                    .ConfigureAwait(false);

                // this is a cheat, but another side-effect of this weird corner case
                _cachedValue = customValueNode.Value;

                return;
            }

            var lazyContext = CreateLazySerializationContext();

            foreach (var child in GetSerializableChildren())
            {
                EmitBeginDeserialization(stream, child, lazyContext, eventShuttle);

                await child.DeserializeAsync(stream, eventShuttle, cancellationToken).ConfigureAwait(false);

                EmitEndDeserialization(stream, child, lazyContext, eventShuttle);
            }
        }

        protected override Type GetValueTypeOverride()
        {
            return _valueType;
        }

        /// <summary>
        ///     Used to get node value with a value selector to select between value and bound value.
        /// </summary>
        /// <param name="childValueSelector"></param>
        /// <returns></returns>
        private object GetValue(Func<ValueNode, object> childValueSelector)
        {
            if (_valueType == null)
            {
                return null;
            }

            // can't do anything if this type is abstract
            if (_valueType.GetTypeInfo().IsAbstract)
            {
                return null;
            }

            var objectTypeNode = (ObjectTypeNode) TypeNode;

            // if ignored, the best we can do is return set value
            if (objectTypeNode.IsIgnored)
            {
                return _cachedValue;
            }

            // make sure we're operating on the correct (possibly sub-) type.
            var node = objectTypeNode.GetSubTypeNode(_valueType);

            // see if it's possible to construct at all
            if (node.Constructor == null)
            {
                throw new InvalidOperationException("No public constructors.");
            }

            // let's figure out the actual value now
            object value;
            IEnumerable<ValueNode> nonparameterizedChildren;

            if (node.ConstructorParameterNames.Length == 0)
            {
                // handle simple case of no parameterized constructors
                value = node.CompiledConstructor();

                nonparameterizedChildren = Children.ToList();
            }
            else
            {
                // find best match for constructor
                var serializableChildren = GetSerializableChildren();

                // find children that can be initialized or partially initialized with construction based on matching name
                var parameterizedChildren = node.ConstructorParameterNames.Join(serializableChildren,
                        parameter => parameter.ToLower(),
                        serializableChild => serializableChild.Name.ToLower(),
                        (parameter, serializableChild) => serializableChild)
                    .ToList();

                // get constructor arguments based on child selector
                var parameterValues = parameterizedChildren.Select(childValueSelector).ToArray();

                // construct our value
                value = node.Constructor.Invoke(parameterValues);

                // get remaining children that weren't used during construction
                nonparameterizedChildren = Children.Except(parameterizedChildren).ToList();
            }

            // set any children not used during construction
            foreach (var child in nonparameterizedChildren)
            {
                var setter = child.TypeNode.ValueSetter;

                if (setter == null && !child.TypeNode.IsIgnored)
                {
                    throw new InvalidOperationException("No public setter.");
                }

                setter?.Invoke(value, childValueSelector(child));
            }

            return value;
        }

        private bool IsCustomNode(out ValueNode customValueNode)
        {
            var parent = (TypeNode) TypeNode.Parent;

            if (_valueType != null &&
                (TypeNode.SubtypeBinding != null || parent.ItemSubtypeBinding != null ||
                 TypeNode.SubtypeFactoryBinding != null || parent.ItemSubtypeFactoryBinding != null))
            {
                var typeNode = (ObjectTypeNode) TypeNode;
                var subType = typeNode.GetSubTypeNode(_valueType);

                if (subType is CustomTypeNode)
                {
                    customValueNode = subType.CreateSerializer((ValueNode) Parent);
                    return true;
                }
            }

            customValueNode = null;

            return false;
        }

        private void EmitEndSerialization(BoundedStream stream, ValueNode child,
            LazyBinarySerializationContext lazyContext,
            EventShuttle eventShuttle)
        {
            if (eventShuttle != null && eventShuttle.HasSerializationSubscribers)
            {
                eventShuttle.OnMemberSerialized(this, child.Name, child.BoundValue, lazyContext,
                    stream.GlobalPosition);
            }
        }

        private void EmitBeginSerialization(BoundedStream stream, ValueNode child,
            LazyBinarySerializationContext lazyContext,
            EventShuttle eventShuttle)
        {
            if (eventShuttle != null && eventShuttle.HasSerializationSubscribers)
            {
                eventShuttle.OnMemberSerializing(this, child.Name, lazyContext,
                    stream.GlobalPosition);
            }
        }

        private void GenerateChildren()
        {
            var typeNode = (ObjectTypeNode) TypeNode;

            // generate correct children for this subtype
            var subType = typeNode.GetSubTypeNode(_valueType);
            Children = new List<ValueNode>(subType.Children.Select(child => child.CreateSerializer(this)));
        }

        private void SkipPadding(BoundedStream stream)
        {
            var length = GetFieldLength();

            if (length != null)
            {
                if (length > stream.RelativePosition)
                {
                    var padLength = length - stream.RelativePosition;
                    var pad = new byte[(int) padLength];
                    stream.Read(pad, 0, pad.Length);
                }
            }
        }

        private void ResolveValueType()
        {
            var parent = (TypeNode) TypeNode.Parent;

            // first check for any immediate subtype information
            if (TypeNode.SubtypeBinding != null || TypeNode.SubtypeFactoryBinding != null ||
                TypeNode.SubtypeDefaultAttribute != null)
            {
                SetValueType(TypeNode.SubtypeBinding, this, TypeNode.SubtypeAttributes,
                    TypeNode.SubtypeFactoryBinding, TypeNode.SubtypeFactory,
                    TypeNode.SubtypeDefaultAttribute);
            }

            // failing that, check for parent subtype information
            if (_valueType == null &&
                (parent.ItemSubtypeBinding != null || parent.ItemSubtypeFactoryBinding != null ||
                 parent.ItemSubtypeDefaultAttribute != null))
            {
                SetValueType(parent.ItemSubtypeBinding, (ValueNode) Parent, parent.ItemSubtypeAttributes,
                    parent.ItemSubtypeFactoryBinding, parent.ItemSubtypeFactory,
                    parent.ItemSubtypeDefaultAttribute);
            }

            // no subtype information
            if (_valueType == null)
            {
                // trivial case with no subtypes
                _valueType = TypeNode.Type;
            }
        }

        private void SetValueType(Binding binding, ValueNode bindingTarget,
            ReadOnlyCollection<SubtypeBaseAttribute> attributes,
            Binding subtypeFactoryBinding,
            ISubtypeFactory subtypeFactory,
            SubtypeDefaultBaseAttribute defaultAttribute)
        {
            if (binding != null)
            {
                // try to resolve value type using subtype mapping
                var subTypeValue = binding.GetValue(bindingTarget);

                // find matching subtype, if available
                var matchingAttribute = attributes.SingleOrDefault(
                    attribute => subTypeValue.Equals(
                        Convert.ChangeType(attribute.Value, subTypeValue.GetType(), null)));

                _valueType = matchingAttribute?.Subtype;
            }

            if (_valueType == null && subtypeFactoryBinding != null)
            {
                var subTypeFactoryValue = subtypeFactoryBinding.GetValue(bindingTarget);

                Type valueType;
                if (subtypeFactory.TryGetType(subTypeFactoryValue, out valueType))
                {
                    _valueType = valueType;
                }
            }

            // we couldn't match so use default if specified
            if (_valueType == null && defaultAttribute != null)
            {
                _valueType = defaultAttribute.Subtype;
            }
        }

        private void EmitEndDeserialization(BoundedStream stream, ValueNode child,
            LazyBinarySerializationContext lazyContext,
            EventShuttle eventShuttle)
        {
            if (eventShuttle != null && eventShuttle.HasDeserializationSubscribers)
            {
                eventShuttle.OnMemberDeserialized(this, child.Name, child.Value, lazyContext,
                    stream.GlobalPosition);
            }
        }

        private void EmitBeginDeserialization(BoundedStream stream, ValueNode child,
            LazyBinarySerializationContext lazyContext,
            EventShuttle eventShuttle)
        {
            if (eventShuttle != null && eventShuttle.HasDeserializationSubscribers)
            {
                eventShuttle.OnMemberDeserializing(this, child.Name, lazyContext,
                    stream.GlobalPosition);
            }
        }
    }
}