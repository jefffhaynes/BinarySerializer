using System;
using System.Text;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies the encoding (string representation) for a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FieldEncodingAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        private readonly Encoding _encoding;

        /// <summary>
        ///     Initializes a new instance of the FieldEncoding class with a fixed encoding.
        /// </summary>
        /// <param name="encodingName">The field encoding name.</param>
        public FieldEncodingAttribute(string encodingName)
        {
            _encoding = Encoding.GetEncoding(encodingName);
            BindingMode = BindingMode.OneWay;
        }


        /// <summary>
        ///     Initializes a new instance of the FieldEncoding class with a path pointing to a binding source member.
        ///     <param name="path">A path to the source member.</param>
        ///     <param name="converterType">
        ///         Gets or sets the type of converter to use.  The specified converter must return a
        ///         valid Encoding.
        ///     </param>
        /// </summary>
        public FieldEncodingAttribute(string path, Type converterType) : base(path)
        {
            ConverterType = converterType;
            BindingMode = BindingMode.OneWay;
        }

        /// <summary>
        ///     Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return _encoding;
        }
    }
}