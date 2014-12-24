using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerialization;

namespace BinarySerialization
{
    internal class ObjectNode : ContainerNode
    {
        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public ObjectNode(Type type) : this(null, type)
        {
        }

        public ObjectNode(Node parent, Type type) : base(parent, type)
        {
            var children = GenerateChildren(type);
            Children.AddRange(children);
        }

        public ObjectNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            if (memberInfo == null)
                return;

            var type = GetMemberType(memberInfo);
            var children = GenerateChildren(type);
            Children.AddRange(children);
        }

        public override object Value
        {
            get
            {
                var value = Activator.CreateInstance(Type);

                foreach (var child in Children)
                    child.ValueSetter(value, child.Value);

                return value;
            }

            set
            {
                if (value == null)
                    return;

                foreach (var child in Children)
                    child.Value = child.ValueGetter(value);
            }
        }

        private IEnumerable<Node> GetSerializableChildren()
        {
            return Children.Where(child => child.ShouldSerialize);
        }

        public override void Serialize(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
                using (new StreamPositioner(stream, child.FieldOffsetBinding))
                    child.Serialize(stream);
        }

        public override void Deserialize(StreamLimiter stream)
        {
            var serializableChildren = GetSerializableChildren();

            if (FieldLengthBinding != null)
                stream = new StreamLimiter(stream, (long) FieldLengthBinding.Value);

            foreach (var child in serializableChildren)
            {
                if (ShouldTerminate(stream))
                    break;

                using (new StreamPositioner(stream, child.FieldOffsetBinding))
                    child.Deserialize(stream);
            }
        }

        private IEnumerable<Node> GenerateChildren(Type type)
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

            return orderedMembers.Select(GenerateChild);
        }
    }
}