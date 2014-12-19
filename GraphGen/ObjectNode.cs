using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using BinarySerialization;

namespace GraphGen
{
    internal class ObjectNode : Node
    {
        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public ObjectNode(Type type) : base(null, type)
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
                foreach (var child in Children)
                    child.Value = child.ValueGetter(value);
            }
        }

        private IEnumerable<Node> GetSerializableChildren()
        {
            return Children.Where(child => !child.Ignore);
        }

        public override void Serialize(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
                child.Serialize(stream);
        }

        public override void Deserialize(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
                child.Deserialize(stream);
        }

        private IEnumerable<Node> GenerateChildren(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);
            return all.Select(GenerateChild);
        }

        private Node GenerateChild(MemberInfo memberInfo)
        {
            var memberType = GetMemberType(memberInfo);

            Node node;
            if (memberType.IsPrimitive || memberType == typeof(string))
                node = new ValueNode(this, memberInfo);
            else node = new ObjectNode(this, memberInfo);

            return node;
        }

        private static Type GetMemberType(MemberInfo memberInfo)
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



        //public static void SetValue(MemberInfo memberInfo, object definingValue, object value)
        //{
        //    var propertyInfo = memberInfo as PropertyInfo;
        //    var fieldInfo = memberInfo as FieldInfo;

        //    if (propertyInfo != null)
        //    {
        //        //var convertedValue = value.ConvertTo(propertyInfo.PropertyType);
        //        //propertyInfo.SetValue(definingValue, convertedValue, null);
        //        propertyInfo.SetValue(definingValue, value, null);
        //        return;
        //    }

        //    if (fieldInfo != null)
        //    {
        //        //var convertedValue = value.ConvertTo(fieldInfo.FieldType);
        //        //fieldInfo.SetValue(definingValue, convertedValue);
        //        fieldInfo.SetValue(definingValue, value);
        //        return;
        //    }

        //    throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));
        //}
    }
}