using System;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class EnumTypeNode : ValueTypeNode
    {
        public EnumTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            InitializeEnumValues();
        }

        public EnumTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType,
            memberInfo)
        {
            InitializeEnumValues();
        }

        public EnumInfo EnumInfo { get; private set; }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new EnumValueNode(parent, Name, this);
        }

        private void InitializeEnumValues()
        {
            var serializedType = GetSerializedType();

            var values = Enum.GetValues(BaseSerializedType).Cast<Enum>();

            /* Get enum attributes */
            var enumAttributes = values.ToDictionary(value => value, value =>
            {
                var memberInfo = BaseSerializedType.GetMember(value.ToString()).Single();
                return (SerializeAsEnumAttribute) memberInfo.GetCustomAttributes(
                    typeof(SerializeAsEnumAttribute),
                    false).FirstOrDefault();
            });

            EnumInfo = new EnumInfo();

            /* If any are specified, build dictionary of them one time */
            if (enumAttributes.Any(enumAttribute => enumAttribute.Value != null) ||
                serializedType == SerializedType.TerminatedString ||
                serializedType == SerializedType.SizedString ||
                serializedType == SerializedType.LengthPrefixedString ||
                serializedType == SerializedType.TerminatedSizedString)
            {
                EnumInfo.EnumValues = enumAttributes.ToDictionary(enumAttribute => enumAttribute.Key,
                    enumAttribute =>
                    {
                        var attribute = enumAttribute.Value;
                        return attribute?.Value ?? enumAttribute.Key.ToString();
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
                    else
                    {
                        EnumInfo.SerializedType = SerializedType.TerminatedString;
                    }
                }
                else if (serializedType == SerializedType.SizedString)
                {
                    /* If fixed size is specified, get max length in order to accommodate all values */
                    EnumInfo.EnumValueLength = lengthGroups[0].Max(lengthGroup => lengthGroup.Length);
                }
            }

            /* If a field length is specified to be less than the max enum value length, we can't reliably recover the enum
             * values on deserialization. */
            //if (enumInfo.EnumValueLength != null && FieldLengthBinding != null && FieldLengthBindings.IsConst)
            //{
            //    if ((int) FieldLengthBinding.Value < enumInfo.EnumValueLength.Value)
            //        throw new InvalidOperationException("Field length cannot be less than max enum name length.");
            //}

            EnumInfo.UnderlyingType = Enum.GetUnderlyingType(BaseSerializedType);

            if (EnumInfo.SerializedType == SerializedType.Default)
            {
                EnumInfo.SerializedType = GetSerializedType(EnumInfo.UnderlyingType);
            }
        }
    }
}