using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to serialize and deserialize enumerations as string fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SerializeAsEnumAttribute : Attribute
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializeAsEnumAttribute"/> class.  Decorating an enum with this
        /// attribute will cause the enum to be serialized as the null-terminated enum name.
        /// </summary>
        public SerializeAsEnumAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializeAsEnumAttribute"/> class, with a string value
        /// to use during serialization.
        /// </summary>
        /// <param name="value">The literal value of the enum when serialized.</param>
        /// <example>
        /// <code>
        /// using BinarySerializer;
        /// 
        /// public enum Shape
        /// {
        ///     [SerializeAsEnum("CIR")]
        ///     Circle,
        ///     
        ///     [SerializeAsEnum("SQR")]
        ///     Square,
        /// 
        ///     [SerializeAsEnum("TRI")]
        ///     Triangle
        /// }
        /// </code>
        /// </example>
        public SerializeAsEnumAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The literal string to use during serialization.
        /// </summary>
        public string Value { get; set; }
    }
}
