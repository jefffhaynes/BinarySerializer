using BinarySerialization.Graph;
using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Provides the <see cref="BinarySerializer" /> with information used to serialize the decorated member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializeAsAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SerializeAsAttribute" /> class.
        /// </summary>
        public SerializeAsAttribute(Type converterType=null, object converterParameter = null)
        {
            ConverterType = converterType;
            ConverterParameter = converterParameter;
        }

        /// <summary>
        ///     Initializes a new instance of the SerializeAs class with a specified <see cref="SerializedType" />.
        /// </summary>
        public SerializeAsAttribute(SerializedType serializedType, Type converterType = null, object converterParameter = null) 
            : this(converterType, converterParameter)
        {
            SerializedType = serializedType;
        }

        /// <summary>
        ///     Specifies the type to which to serialize the member.
        /// </summary>
        public SerializedType SerializedType { get; set; }

        /// <summary>
        /// Specifies the string terminator when the serialized type is TerminatedString.  Null (zero) by default.
        /// </summary>
        public char StringTerminator { get; set; }

        /// <summary>
        /// Specifies padding value to be used when serializing fixed-size fields.
        /// </summary>
        public byte PaddingValue { get; set; }
        /// <summary>
        ///     An optional converter to be used converting from the source value to the target binding.
        /// </summary>
        public Type ConverterType { get; set; }

        /// <summary>
        ///     An optional converter parameter to be passed to the converter.
        /// </summary>
        public object ConverterParameter { get; set; }
    }
}