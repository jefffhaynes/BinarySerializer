using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal class ObjectNode : ContainerNode
    {
        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public;

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
                    if (value != null)
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

        public ObjectNode(Type type) : this(null, type)
        {
        }

        public ObjectNode(Node parent, Type type) : base(parent, type)
        {
            GenerateChildren(type);
            ValueType = type;
        }

        public ObjectNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            if (memberInfo == null)
                return;

            var type = GetMemberType(memberInfo);
            GenerateChildren(type);

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
                if (SubtypeBinding != null && SubtypeBinding.Value != null)
                {
                    var matchingAttribute =
                        SubtypeAttributes.SingleOrDefault(attribute => attribute.Value.Equals(SubtypeBinding.Value));

                    /* If we can't find a match, default our value to null */
                    if (matchingAttribute == null)
                        return null;

                    ValueType = matchingAttribute.Subtype;
                }
                else ValueType = Type;

                var value = Activator.CreateInstance(ValueType);

                foreach (var child in Children)
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

            foreach (var child in serializableChildren)
            {
                OnMemberSerializing();
                using (new StreamResetter(stream, child.FieldOffsetBinding != null))
                {
                    if (child.FieldOffsetBinding != null)
                        stream.Position = (long)child.FieldOffsetBinding.Value;

                    child.Serialize(stream);
                }
                OnMemberSerialized();
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            var serializableChildren = GetSerializableChildren();

            if (FieldLengthBinding != null)
                stream = new StreamLimiter(stream, (long) FieldLengthBinding.Value);

            foreach (var child in serializableChildren.TakeWhile(child => !ShouldTerminate(stream)))
            {
                OnMemberDeserializing();
                using (new StreamResetter(stream, child.FieldOffsetBinding != null))
                {
                    if (child.FieldOffsetBinding != null)
                        stream.Position = (long) child.FieldOffsetBinding.Value;

                    child.Deserialize(stream);
                }
                OnMemberDeserialized();
            }
        }

        private void GenerateChildren(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            /* Because binding happens during construction, we have to check ordering here */
            var orderedMembers = all.Select(member =>
            {
                var serializeAsAttribute = member.GetAttribute<SerializeAsAttribute>();
                var order = serializeAsAttribute != null ? serializeAsAttribute.Order : 0;
                return new KeyValuePair<MemberInfo, int>(member, order);
            }).OrderBy(keyValue => keyValue.Value).Select(keyValue => keyValue.Key);

            var children = orderedMembers.Select(GenerateChild).ToList();

            _typeChildren.Add(type, children);
        }

        protected override Type GetValueTypeOverride()
        {
            return ValueType;
        }
    }
}