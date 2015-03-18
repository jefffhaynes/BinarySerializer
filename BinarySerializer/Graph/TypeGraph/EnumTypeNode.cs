using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    public class EnumTypeNode : ValueTypeNode
    {
        public EnumTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            InitializeEnumValues();
        }

        public EnumTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            InitializeEnumValues();
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new EnumValueNode(parent, Name, this);
        }

        public EnumInfo EnumInfo { get; private set; }

        public IEnumerable<Enum> UnderlyingEnumValues
        {
            get { return Enum.GetValues(Type).Cast<Enum>(); }
        }

        private void InitializeEnumValues()
        {
            var serializedType = GetSerializedType();

            /* Get enum attributes */
            var enumAttributes = UnderlyingEnumValues.ToDictionary(value => value, value =>
            {
                var memberInfo = Type.GetMember(value.ToString()).Single();
                return (SerializeAsEnumAttribute) memberInfo.GetCustomAttributes(
                    typeof (SerializeAsEnumAttribute),
                    false).FirstOrDefault();
            });

            EnumInfo = new EnumInfo();

            /* If any are specified, build dictionary of them one time */
            if (enumAttributes.Any(enumAttribute => enumAttribute.Value != null) ||
                serializedType == SerializedType.NullTerminatedString ||
                serializedType == SerializedType.SizedString ||
                serializedType == SerializedType.LengthPrefixedString)
            {
                EnumInfo.EnumValues = enumAttributes.ToDictionary(enumAttribute => enumAttribute.Key,
                    enumAttribute =>
                    {
                        var attribute = enumAttribute.Value;
                        return attribute != null && attribute.Value != null
                            ? attribute.Value
                            : enumAttribute.Key.ToString();
                    });

                EnumInfo.ValueEnums = EnumInfo.EnumValues.ToDictionary(enumValue => enumValue.Value,
                    enumValue => enumValue.Key);

                var lengthGroups =
                    EnumInfo.EnumValues.Where(enumValue => enumValue.Value != null)
                        .Select(enumValue => enumValue.Value)
                        .GroupBy(value => value.Length).ToList();


                /* If the graphType isn't specified, let's try to guess it smartly */
                if (serializedType == SerializedType.Default)
                {
                    /* If everything is the same length, assume fixed length */
                    if (lengthGroups.Count == 1)
                    {
                        EnumInfo.SerializedType = SerializedType.SizedString;
                        EnumInfo.EnumValueLength = lengthGroups[0].Key;
                    }
                    else EnumInfo.SerializedType = SerializedType.NullTerminatedString;
                }
                else if (serializedType == SerializedType.SizedString)
                {
                    /* If fixed size is specified, get max length in order to accomodate all values */
                    EnumInfo.EnumValueLength = lengthGroups[0].Max(lengthGroup => lengthGroup.Length);
                }
            }

            /* If a field length is specified to be less than the max enum value length, we can't reliably recover the enum
             * values on deserialization. */
            //if (enumInfo.EnumValueLength != null && FieldLengthBinding != null && FieldLengthBinding.IsConst)
            //{
            //    if ((int) FieldLengthBinding.Value < enumInfo.EnumValueLength.Value)
            //        throw new InvalidOperationException("Field length cannot be less than max enum name length.");
            //}

            EnumInfo.UnderlyingType = Enum.GetUnderlyingType(Type);

            if (EnumInfo.SerializedType == SerializedType.Default)
            {
                EnumInfo.SerializedType = GetSerializedType(EnumInfo.UnderlyingType);
            }
        }
    }
}