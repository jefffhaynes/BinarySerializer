using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace BinarySerialization
{
    internal abstract class ContainerNode : Node
    {
        protected ContainerNode(Node parent, Type type)
            : base(parent, type)
        {
        }

        protected ContainerNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        protected Node GenerateChild(Type type)
        {
            ThrowOnBadType(type);

            var nodeType = GetNodeType(type);
            var node = (Node) Activator.CreateInstance(nodeType, this, type);
            return node;
        }

        protected Node GenerateChild(MemberInfo memberInfo)
        {
            var memberType = GetMemberType(memberInfo);

            ThrowOnBadType(memberType);

            var nodeType = GetNodeType(memberType);
            return Activator.CreateInstance(nodeType, this, memberInfo) as Node;
        }

// ReSharper disable UnusedParameter.Local
        private static void ThrowOnBadType(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
                throw new InvalidOperationException("Cannot serialize objects that implement IDictionary.");
        }
// ReSharper restore UnusedParameter.Local

        private static Type GetNodeType(Type type)
        {
            if (type.IsEnum)
                return typeof (EnumNode);
            if (type.IsPrimitive || type == typeof (string) || type == typeof (byte[]))
                return typeof (ValueNode);
            if (Nullable.GetUnderlyingType(type) != null)
                return typeof (ValueNode);
            if (type.IsArray)
                return typeof (ArrayNode);
            if (type.IsList())
                return typeof (ListNode);
            if (typeof (Stream).IsAssignableFrom(type))
                return typeof (StreamNode);
            return typeof (ObjectNode);
        }

        protected static Type GetMemberType(MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;
            
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));
        }

        protected static bool ShouldTerminate(StreamLimiter stream)
        {
            if (stream.IsAtLimit)
                return true;

            return stream.CanSeek && stream.Position >= stream.Length;
        }
    }
}