using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal class EnumNode : ValueNode
    {
        private static readonly IDictionary<Type, EnumInfo> EnumTypeInfo = new Dictionary<Type, EnumInfo>();
        private static object EnumTypeInfoLock = new object();

        public EnumNode(Node parent, Type type) : base(parent, type)
        {
            InitializeEnumValues();
        }

        public EnumNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            InitializeEnumValues();
        }

        public override void SerializeOverride(Stream stream)
        {
            EnumInfo enumInfo = EnumTypeInfo[Type];
            var value = enumInfo.EnumValues != null ? enumInfo.EnumValues[(Enum)BoundValue] : BoundValue;
            Serialize(stream, value, enumInfo.SerializedType, enumInfo.EnumValueLength);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            EnumInfo enumInfo = EnumTypeInfo[Type];
            var value = Deserialize(stream, enumInfo.SerializedType, enumInfo.EnumValueLength);

            if (enumInfo.ValueEnums != null)
            {
                value = enumInfo.ValueEnums[(string) value];
            }

            Func<object, object> converter;
            var underlyingValue = TypeConverters.TryGetValue(enumInfo.UnderlyingType, out converter)
                ? converter(value)
                : value;

            Value = Enum.ToObject(Type, underlyingValue);
        }

        private void InitializeEnumValues()
        {
            if (EnumTypeInfo.ContainsKey(Type))
                return;

            lock (EnumTypeInfoLock)
            {
                if (EnumTypeInfo.ContainsKey(Type))
                    return;

                var serializedType = GetSerializedType();

                var values = Enum.GetValues(Type).Cast<Enum>();

                /* Get enum attributes */
                var enumAttributes = values.ToDictionary(value => value, value =>
                {
                    var memberInfo = Type.GetMember(value.ToString()).Single();
                    return (SerializeAsEnumAttribute) memberInfo.GetCustomAttributes(
                        typeof (SerializeAsEnumAttribute),
                        false).FirstOrDefault();
                });

                var enumInfo = new EnumInfo();

                /* If any are specified, build dictionary of them one time */
                if (enumAttributes.Any(enumAttribute => enumAttribute.Value != null))
                {
                    enumInfo.EnumValues = enumAttributes.ToDictionary(enumAttribute => enumAttribute.Key,
                        enumAttribute =>
                        {
                            var attribute = enumAttribute.Value;
                            return attribute != null && attribute.Value != null
                                ? attribute.Value
                                : enumAttribute.Key.ToString();
                        });

                    enumInfo.ValueEnums = enumInfo.EnumValues.ToDictionary(enumValue => enumValue.Value,
                        enumValue => enumValue.Key);

                    var lengthGroups =
                        enumInfo.EnumValues.Where(enumValue => enumValue.Value != null)
                            .Select(enumValue => enumValue.Value)
                            .GroupBy(value => value.Length).ToList();


                    /* If the graphType isn't specified, let's try to guess it smartly */
                    if (serializedType == SerializedType.Default)
                    {
                        /* If everything is the same length, assume fixed length */
                        if (lengthGroups.Count == 1)
                        {
                            enumInfo.SerializedType = SerializedType.SizedString;
                            enumInfo.EnumValueLength = lengthGroups[0].Key;
                        }
                        else enumInfo.SerializedType = SerializedType.NullTerminatedString;
                    }
                    else if (serializedType == SerializedType.SizedString)
                    {
                        /* If fixed size is specified, get max length in order to accomodate all values */
                        enumInfo.EnumValueLength = lengthGroups[0].Max(lengthGroup => lengthGroup.Length);
                    }
                }

                /* If a field length is specified to be less than the max enum value length, we can't reliably recover the enum
             * values on deserialization. */
                if (enumInfo.EnumValueLength != null && FieldLengthBinding != null && FieldLengthBinding.IsConst)
                {
                    if ((int) FieldLengthBinding.Value < enumInfo.EnumValueLength.Value)
                        throw new InvalidOperationException("Field length cannot be less than max enum name length.");
                }

                enumInfo.UnderlyingType = Enum.GetUnderlyingType(Type);

                if (enumInfo.SerializedType == SerializedType.Default)
                {
                    enumInfo.SerializedType = GetSerializedType(enumInfo.UnderlyingType);
                }

                EnumTypeInfo.Add(Type, enumInfo);
            }
        }
    }
}