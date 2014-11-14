using System;
using System.Reflection;

namespace BinarySerialization
{
    internal class EnumMemberSerializationInfo
    {
        public EnumMemberSerializationInfo(MemberInfo member)
        {
            Member = member;
        }

        public EnumMemberSerializationInfo(MemberInfo member, SerializeAsEnumAttribute enumAttribute, Enum value)
            : this(member)
        {
            SerializeAsEnumAttribute = enumAttribute;
            Value = value;
        }

        public MemberInfo Member { get; set; }
        public SerializeAsEnumAttribute SerializeAsEnumAttribute { get; set; }
        public Enum Value { get; set; }
    }
}