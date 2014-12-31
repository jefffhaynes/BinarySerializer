using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal class ObjectNode : ContainerNode
    {
        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly Dictionary<Type, List<Node>> _typeChildren = new Dictionary<Type, List<Node>>();

        private Type _valueType;

        public Type ValueType
        {
            get { return _valueType; }

            private set
            {
                if (_valueType != value)
                {
                    ClearChildren();
                    if (value != null && !Ignore)
                    {
                        List<Node> children;
                        if (!_typeChildren.TryGetValue(value, out children))
                        {
                            /* Previously unseen type */
                            GenerateChildren(value);
                            children = _typeChildren[value];
                        }

                        AddChildren(children);
                    }
                    _valueType = value;
                }
            }
        }

        public ObjectNode(Node parent, Type type) : base(parent, type)
        {
            ValueType = type;
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

            ValueType = type;

            if (SubtypeAttributes.Length <= 0) 
                return;

            var subTypes = SubtypeAttributes.Select(attribute => attribute.Subtype);

            foreach(var subType in subTypes)
                GenerateChildren(subType);
        }

        public override object Value
        {
            get
            {
                var valueType = ResolveValueType();
                if (valueType == null)
                    return null;

                ValueType = valueType;

                var value = Activator.CreateInstance(ValueType);

                foreach (var child in Children.Where(child => !child.Ignore))
                    child.ValueSetter(value, child.Value);

                return value;
            }

            set
            {
                if (value == null)
                {
                    ValueType = null;
                    return;
                }

                ValueType = value.GetType();

                foreach (var child in Children)
                    child.Value = child.ValueGetter(value);
            }
        }

        private IEnumerable<Node> GetSerializableChildren()
        {
            return Children.Where(child => child.ShouldSerialize);
        }

        public override void SerializeOverride(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            var serializationContext = CreateSerializationContext();
            foreach (var child in serializableChildren)
            {
                OnMemberSerializing(this, new MemberSerializingEventArgs(child.Name, serializationContext));
                using (new StreamResetter(stream, child.FieldOffsetBinding != null))
                {
                    if (child.FieldOffsetBinding != null)
                        stream.Position = (long)child.FieldOffsetBinding.Value;

                    child.Serialize(stream);
                }
                OnMemberSerialized(this, new MemberSerializedEventArgs(child.Name, child.BoundValue, serializationContext));
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
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
                }
                OnMemberDeserialized(this, new MemberSerializedEventArgs(child.Name, child.Value, serializationContext));
            }
        }

        private void GenerateChildren(Type type)
        {
            //var members = type.GetMembers(MemberBindingFlags);
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            //IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            //IEnumerable<MemberInfo> all = properties.Union(fields);

            /* Because binding happens during construction, we have to check ordering here */
            var orderedMembers = properties.Select(member =>
            {
                var serializeAsAttribute = member.GetAttribute<SerializeAsAttribute>();
                var order = serializeAsAttribute != null ? serializeAsAttribute.Order : 0;
                return new KeyValuePair<MemberInfo, int>(member, order);
            }).OrderBy(keyValue => keyValue.Value).Select(keyValue => keyValue.Key);

            var children = orderedMembers.Select(GenerateChild).ToList();

            _typeChildren.Add(type, children);
        }

        private Type ResolveValueType()
        {
            if (SubtypeBinding == null || SubtypeBinding.Value == null)
                return Type;

            var matchingAttribute =
                SubtypeAttributes.SingleOrDefault(attribute => attribute.Value.Equals(SubtypeBinding.Value));

            /* If we can't find a match, default our value to null */
            return matchingAttribute == null ? null : matchingAttribute.Subtype;
        }

        protected override Type GetValueTypeOverride()
        {
            return ValueType;
        }
    }
}