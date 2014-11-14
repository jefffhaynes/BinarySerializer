using System.Reflection;

namespace BinarySerialization
{
    internal class MemberSerializationInfo
    {
        public MemberSerializationInfo(MemberInfo member)
        {
            Member = member;
        }

        public MemberSerializationInfo(MemberInfo member, SerializeAsAttribute serializeAsAttribute,
                                       IgnoreAttribute ignoreAttribute, FieldOffsetAttribute fieldOffsetAttribute,
                                       FieldLengthAttribute fieldLengthAttribute,
                                       FieldCountAttribute fieldCountAttribute,
                                       SerializeWhenAttribute serializeWhenAttribute,
                                       SerializeUntilAttribute serializeUntilAttribute,
                                       ItemLengthAttribute itemLengthAttribute,
                                       ItemSerializeUntilAttribute itemSerializeUntilAttribute,
                                       SubtypeAttribute[] subtypeAttributes)
            : this(member)
        {
            SerializeAsAttribute = serializeAsAttribute;
            IgnoreAttribute = ignoreAttribute;
            FieldOffsetAttribute = fieldOffsetAttribute;
            FieldLengthAttribute = fieldLengthAttribute;
            FieldCountAttribute = fieldCountAttribute;
            SerializeWhenAttribute = serializeWhenAttribute;
            SerializeUntilAttribute = serializeUntilAttribute;
            ItemLengthAttribute = itemLengthAttribute;
            ItemSerializeUntilAttribute = itemSerializeUntilAttribute;
            SubtypeAttributes = subtypeAttributes;
        }

        public MemberInfo Member { get; set; }
        public SerializeAsAttribute SerializeAsAttribute { get; set; }
        public IgnoreAttribute IgnoreAttribute { get; set; }
        public FieldOffsetAttribute FieldOffsetAttribute { get; set; }
        public FieldLengthAttribute FieldLengthAttribute { get; set; }
        public FieldCountAttribute FieldCountAttribute { get; set; }
        public SerializeWhenAttribute SerializeWhenAttribute { get; set; }
        public SerializeUntilAttribute SerializeUntilAttribute { get; set; }
        public ItemLengthAttribute ItemLengthAttribute { get; set; }
        public ItemSerializeUntilAttribute ItemSerializeUntilAttribute { get; set; }
        public SubtypeAttribute[] SubtypeAttributes { get; set; }
    }
}