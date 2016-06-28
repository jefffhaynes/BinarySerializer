using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to specify multiple possible derived types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=true)]
    public sealed class SubtypeAttribute : FieldBindingBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtypeAttribute"/>.
        /// </summary>
        /// <param name="valuePath">The path to the binding source.</param>
        /// <param name="value">The value to be used in determining if the subtype should be used.</param>
        /// <param name="subtype">The subtype to be used.</param>
        public SubtypeAttribute(string valuePath, object value, Type subtype)
            : base(valuePath)
        {
            Value = value;
            Subtype = subtype;
        }
		
        /// <summary>
        /// The value that defines the subtype mapping.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// The subtype.
        /// </summary>
        public Type Subtype { get; private set; }
    }
}
