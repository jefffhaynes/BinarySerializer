using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.TypeGraph
{
    internal class ObjectNode : ContainerNode
    {
        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly Dictionary<Type, List<Node>> _typeChildren = new Dictionary<Type, List<Node>>();
        private readonly object _typeChildrenLock = new object();

        //private Type _valueType;
        //private Type _setValueType;

        //private bool _isCacheDirty = true;
        //private object _cachedValue;

        public ObjectNode(Node parent, Type type) : base(parent, type)
        {
        }

        public ObjectNode(Type type)
            : this(null, type)
        {
        }

        public ObjectNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            if (memberInfo == null)
                return;

            var type = GetMemberType(memberInfo);

            var baseChild = GenerateChild(type);

            AddChild(baseChild);

            /* Add subtypes, if any */
            if (SubtypeAttributes == null || SubtypeAttributes.Count <= 0) 
                return;

            var subTypes = SubtypeAttributes.Select(attribute => attribute.Subtype);


            foreach(var subType in subTypes)
                GenerateChildren(subType);
        }

        //public override object Value
        //{
        //    get
        //    {
        //        var valueType = ResolveValueType();
        //        if (valueType == null)
        //            return null;

        //        ValueType = valueType;

        //        if (!_isCacheDirty)
        //            return _cachedValue;

        //        if (ValueType.IsAbstract)
        //            return null;

        //        var value = Activator.CreateInstance(ValueType);

        //        foreach (var child in Children.Where(child => !child.Ignore))
        //            child.ValueSetter(value, child.Value);

        //        _cachedValue = value;
        //        _isCacheDirty = false;

        //        return value;
        //    }

        //    set
        //    {
        //        _cachedValue = value;
        //        _isCacheDirty = false;

        //        if (value == null)
        //        {
        //            _setValueType = null;
        //            ValueType = null;
        //            return;
        //        }

        //        _setValueType = value.GetType();

        //        ValueType = _setValueType;

        //        UpdateSource(ValueType);

        //        foreach (var child in Children)
        //            child.Value = child.ValueGetter(value);
        //    }
        //}

        private IEnumerable<Node> GetSerializableChildren()
        {
            return Children.Where(child => child.ShouldSerialize);
        }

        public override void SerializeOverride(Stream stream, object value)
        {
            var serializableChildren = GetSerializableChildren();

            //var serializationContext = CreateSerializationContext();
            foreach (var child in serializableChildren)
            {
                //OnMemberSerializing(this, new MemberSerializingEventArgs(child.Name, serializationContext));
                using (new StreamResetter(stream, child.FieldOffsetBinding != null))
                {
                    if (child.FieldOffsetBinding != null)
                        stream.Position = (long)child.FieldOffsetBinding.Value;

                    child.Serialize(stream);
                }
                //OnMemberSerialized(this, new MemberSerializedEventArgs(child.Name, child.BoundValue, serializationContext));
            }
        }

        public override object DeserializeOverride(StreamLimiter stream)
        {
            ClearCache();

            var valueType = ResolveValueType();
            if (valueType == null)
                return;

            ValueType = valueType;

            foreach (var child in Children)
                child.Value = null;

            var serializableChildren = GetSerializableChildren();

            if (FieldLengthBinding != null)
                stream = new StreamLimiter(stream, (long) FieldLengthBinding.Value);

            var serializationContext = CreateSerializationContext();
            foreach (var child in serializableChildren.TakeWhile(child => !ShouldTerminate(stream)))
            {
                OnMemberDeserializing(this, new MemberSerializingEventArgs(child.Name, serializationContext));
                using (new StreamResetter(stream, child.FieldOffsetBinding != null))
                {
                    if (child.FieldOffsetBinding != null)
                        stream.Position = (long) child.FieldOffsetBinding.Value;

                    child.Deserialize(stream);

                    ClearCache();
                }
                OnMemberDeserialized(this, new MemberSerializedEventArgs(child.Name, child.Value, serializationContext));
            }
        }

        private void ClearCache()
        {
            _isCacheDirty = true;
        }

        private void GenerateChildren(Type type)
        {
            lock (_typeChildrenLock)
            {
                if (_typeChildren.ContainsKey(type))
                    return;

                var children = GenerateChildrenImpl(type);
                _typeChildren.Add(type, children.ToList());
            }
        }

        private IEnumerable<Node> GenerateChildrenImpl(Type type)
        { 
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            var children = all.Select(GenerateChild).OrderBy(child => child.Order).ToList();

            var serializableChildren = children.Where(child => !child.Ignore).ToList();

            if (serializableChildren.Count > 1)
            {
                var unorderedChild = serializableChildren.FirstOrDefault(child => child.Order == null);

                if (unorderedChild != null)
                    throw new InvalidOperationException(
                        string.Format(
                            "'{0}' does not have a FieldOrder attribute.  " + 
                            "All serializable fields or properties in a class with more than one member must specify a FieldOrder attribute.",
                            unorderedChild.Name));

                var orderGroups = serializableChildren.GroupBy(child => child.Order);

                if (orderGroups.Count() != serializableChildren.Count)
                    throw new InvalidOperationException("All fields must have a unique order number.");
            }

            if (type.BaseType != null)
            {
                var baseChildren = GenerateChildrenImpl(type.BaseType);
                return baseChildren.Concat(children);
            }

            return children;
        }

        private Type ResolveValueType()
        {
            if (SubtypeBinding == null || SubtypeBinding.Value == null)
            {
                return _setValueType ?? Type;
            }

            var source = (ValueNode)SubtypeBinding.GetSource();
            var bindingValue = SubtypeBinding.Value;

            var matchingAttribute =
                SubtypeAttributes.SingleOrDefault(
                    attribute => source.ConvertToFieldType(attribute.Value).Equals(bindingValue));

            /* If we can't find a match, default our value to null */
            return matchingAttribute == null ? null : matchingAttribute.Subtype;
        }

        private void UpdateSource(Type valueType)
        {
            if (SubtypeBinding == null || SubtypeBinding.Value == null)
                return;

            List<SubtypeAttribute> matchingSubtypes =
                         SubtypeAttributes.Where(attribute => attribute.Subtype == valueType).ToList();

            if (!matchingSubtypes.Any())
            {
                /* Try to fall back on base types */
                matchingSubtypes =
                    SubtypeAttributes.Where(attribute => attribute.Subtype.IsAssignableFrom(valueType))
                        .ToList();

                if (!matchingSubtypes.Any())
                    throw new BindingException("No matching subtype.");
            }

            if (matchingSubtypes.Count() > 1)
                throw new BindingException("Subtypes must have unique types.");

            var value = matchingSubtypes.Single().Value;
            var source = (ValueNode)SubtypeBinding.GetSource();
            source.Value = source.ConvertToFieldType(value);
        }

        protected override Type GetValueTypeOverride()
        {
            return ValueType;
        }
    }
}