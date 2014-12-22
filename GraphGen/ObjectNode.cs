using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerialization;

namespace GraphGen
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
                if(value == null)
                    throw new NotImplementedException("Value cannot be null.");

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
                using (new StreamPositioner(stream, child.FieldOffsetEvaluator))
                    child.Serialize(stream);
        }

        public override void Deserialize(StreamLimiter stream)
        {
            var serializableChildren = GetSerializableChildren();

            if (FieldLengthEvaluator != null)
                stream = new StreamLimiter(stream, (long) FieldLengthEvaluator.Value);

            foreach (var child in serializableChildren)
            {
                if (ShouldTerminate(stream))
                    break;

                using (new StreamPositioner(stream, child.FieldOffsetEvaluator))
                    child.Deserialize(stream);
            }
        }

        private IEnumerable<Node> GenerateChildren(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);
            return all.Select(GenerateChild);
        }
    }
}