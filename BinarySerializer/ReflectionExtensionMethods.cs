using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal static class ReflectionExtensionMethods
    {
        public static object GetValue(this MemberInfo memberInfo, object o)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;

            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(o, null);
            }

            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(o);
            }

            throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));
        }

        public static void SetValue(this MemberInfo memberInfo, object o, object value)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;

            if (propertyInfo != null)
            {
                var convertedValue = value.ConvertTo(propertyInfo.PropertyType);
                propertyInfo.SetValue(o, convertedValue, null);
                return;
            }

            if (fieldInfo != null)
            {
                var convertedValue = value.ConvertTo(fieldInfo.FieldType);
                fieldInfo.SetValue(o, convertedValue);
                return;
            }

            throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
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

        public static T GetAttribute<T>(this Type type)
        {
            return (T)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
        }

        public static T GetAttribute<T>(this MemberInfo type)
        {
            return (T)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
        }

        public static bool IsList(this Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static bool IsByteArray(this Type type)
        {
            return type == typeof (byte[]);
        }

        public static object ConvertTo(this object value, Type valueType)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (type == valueType)
                return value;

            /* Special handling for strings */
            if (type == typeof(string) && valueType.IsPrimitive)
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    value = 0;
            }

            if (valueType == typeof(char))
                return Convert.ToChar(value);
            if (valueType == typeof(Byte))
                return Convert.ToByte(value);
            if (valueType == typeof(SByte))
                return Convert.ToSByte(value);
            if (valueType == typeof(bool))
                return Convert.ToBoolean(value);
            if (valueType == typeof(Int16))
                return Convert.ToInt16(value);
            if (valueType == typeof(Int32))
                return Convert.ToInt32(value);
            if (valueType == typeof(Int64))
                return Convert.ToInt64(value);
            if (valueType == typeof(UInt16))
                return Convert.ToUInt16(value);
            if (valueType == typeof(UInt32))
                return Convert.ToUInt32(value);
            if (valueType == typeof(UInt64))
                return Convert.ToUInt64(value);
            if (valueType == typeof(Single))
                return Convert.ToSingle(value);
            if (valueType == typeof(Double))
                return Convert.ToDouble(value);
            if (valueType == typeof(string))
                return Convert.ToString(value);
            if (valueType.IsEnum && value.GetType().IsPrimitive)
                return Enum.ToObject(valueType, value);

            return value;
        }

        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public static IEnumerable<MemberSerializationInfo> GetMembersSerializationInfo(this Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);

            IEnumerable<MemberInfo> all = properties.Union(fields);

            IEnumerable<MemberSerializationInfo> membersSerializationInfo =
                all.Select(m =>
                {
                    var attributes = m.GetCustomAttributes(true);

                    // TODO probably a better way to do this
                    return new MemberSerializationInfo(m,
                                                       attributes.OfType<SerializeAsAttribute>().SingleOrDefault(),
                                                       attributes.OfType<IgnoreAttribute>().SingleOrDefault(),
                                                       attributes.OfType<FieldOffsetAttribute>().SingleOrDefault(),
                                                       attributes.OfType<FieldLengthAttribute>().SingleOrDefault(),
                                                       attributes.OfType<FieldCountAttribute>().SingleOrDefault(),
                                                       attributes.OfType<SerializeWhenAttribute>().ToArray(),
                                                       attributes.OfType<SerializeUntilAttribute>().SingleOrDefault(),
                                                       attributes.OfType<ItemLengthAttribute>().SingleOrDefault(),
                                                       attributes.OfType<ItemSerializeUntilAttribute>().SingleOrDefault(),
                                                       attributes.OfType<SubtypeAttribute>().ToArray());
                });

            var orderedMembers = membersSerializationInfo.OrderBy(info => info, new MemberSerializationInfoComparer()).ToList();

            var lastMember = orderedMembers.LastOrDefault(member => member.IgnoreAttribute == null);

            if (lastMember != null)
                lastMember.IsLastMember = true;

            return orderedMembers;
        }

        public static IEnumerable<EnumMemberSerializationInfo> GetEnumMembersSerializationInfo(this Type type)
        {
            var values = Enum.GetValues(type).Cast<Enum>();
            return values.Select(value =>
                {
                    var memberInfo = type.GetMember(value.ToString()).Single();
                    var serializeAsEnumAttribute = (SerializeAsEnumAttribute) memberInfo.GetCustomAttributes(
                                                                                      typeof (SerializeAsEnumAttribute),
                                                                                      false).FirstOrDefault();
                    return new EnumMemberSerializationInfo(memberInfo, serializeAsEnumAttribute, value);
                });
        }


        public static IDictionary<Enum, object> GetEnumSerializedValues(this Type enumType, out SerializedType serializedType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("Must be an enum type.", "enumType");

            var memberSerializationInfo = enumType.GetEnumMembersSerializationInfo();

            var enumAttributes = memberSerializationInfo.ToDictionary(
                memberInfo => memberInfo.Value,
                memberInfo => memberInfo.SerializeAsEnumAttribute);

            var hasEnumAttributes = enumAttributes.Consensus(enumAttributePair => enumAttributePair.Value != null);

            if (!hasEnumAttributes.HasValue)
            {
                throw new InvalidOperationException(
                    "If the SerializeAsEnumAttribute is used on one enum value it must be used on all.");
            }

            if (hasEnumAttributes.Value)
            {
                serializedType = SerializedType.NullTerminatedString;
                return enumAttributes.ToDictionary(enumAttributePair => enumAttributePair.Key,
                                                   enumAttributePair =>
                                                   (object)enumAttributePair.Value.Value ??
                                                   (object)enumAttributePair.Key.ToString());
            }

            serializedType = SerializedType.Default;
            return enumAttributes.ToDictionary(enumAttributePair => enumAttributePair.Key,
                                               enumAttributePair => (object)enumAttributePair.Key);
        }

        public static IDictionary<object, Enum> GetValuesForSerializedEnum(this Type enumType)
        {
            SerializedType serializedType;
            return enumType.GetEnumSerializedValues(out serializedType).
                ToDictionary(enumValue => enumValue.Value, enumValue => enumValue.Key);
        }
    }
}
