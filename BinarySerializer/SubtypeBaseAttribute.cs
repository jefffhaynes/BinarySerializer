using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Used to specify multiple possible derived types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class SubtypeBaseAttribute : FieldBindingBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="SubtypeBaseAttribute" />.
        /// </summary>
        /// <param name="valuePath">The path to the binding source.</param>
        /// <param name="value">The value to be used in determining if the subtype should be used.</param>
        /// <param name="subtype">The subtype to be used.</param>
        protected SubtypeBaseAttribute(string valuePath, object value, Type subtype)
            : base(valuePath)
        {
            Value = value;
            Subtype = subtype;
        }

        /// <summary>
        ///     The value that defines the subtype mapping.
        /// </summary>
        public object Value { get; }

        /// <summary>
        ///     The subtype.
        /// </summary>
        public Type Subtype { get; }
    }
}