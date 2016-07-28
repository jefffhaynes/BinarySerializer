using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies the endianness (value byte order) for a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FieldEndiannessAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        private readonly Endianness _endianness;

        /// <summary>
        /// Initializes a new instance of the FieldEndianness class with a constant endianness.
        /// </summary>
        /// <param name="endianness">The field endianness.</param>
        public FieldEndiannessAttribute(Endianness endianness)
        {
            _endianness = endianness;
        }


        /// <summary>
        /// Initializes a new instance of the FieldEndianness class with a path pointing to a binding source member.
        /// <param name="path">A path to the source member.</param> 
        /// <param name="converterType"> Gets or sets the type of converter to use.  The specified converter must return a valid Endianness.</param> 
        /// </summary>
        public FieldEndiannessAttribute(string path, Type converterType) : base(path)
        {
            ConverterType = converterType;
        }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return _endianness;
        }
    }
}
