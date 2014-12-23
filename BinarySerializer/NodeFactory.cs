//using System;
//using System.Reflection;

//namespace BinarySerialization
//{
//    internal static class NodeFactory
//    {
//        public static Node GenerateNode(Node parent, Type type)
//        {
//            Node node;
//            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[]))
//                node = new ValueNode(parent, type);
//            else if (type.IsArray)
//                node = new ArrayNode(parent, type);
//            else if (type.IsList())
//                node = new ListNode(parent, type);
//            else node = new ObjectNode(parent, type);

//            return node;
//        }

//        public static Node GenerateChild(Node parent, MemberInfo memberInfo)
//        {
//            var memberType = GetMemberType(memberInfo);

//            Node node;
//            if (memberType.IsPrimitive || memberType == typeof(string) || memberType == typeof(byte[]))
//                node = new ValueNode(parent, memberInfo);
//            else if (memberType.IsArray)
//                node = new ArrayNode(parent, memberInfo);
//            else if (memberType.IsList())
//                node = new ListNode(parent, memberInfo);
//            else node = new ObjectNode(parent, memberInfo);

//            return node;
//        }

//        private static Type GetMemberType(MemberInfo memberInfo)
//        {
//            var propertyInfo = memberInfo as PropertyInfo;
//            var fieldInfo = memberInfo as FieldInfo;

//            if (propertyInfo != null)
//            {
//                return propertyInfo.PropertyType;
//            }

//            if (fieldInfo != null)
//            {
//                return fieldInfo.FieldType;
//            }

//            throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));
//        }
//    }
//}
