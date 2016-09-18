using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to control conditional serialization of members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=true)]
    public abstract class ConditionalAttribute : FieldBindingBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalAttribute"/>.
        /// </summary>
        /// <param name="valuePath">The path to the binding source.</param>
        /// <param name="value">The value to be used in determining if the condition is true.</param>
        protected ConditionalAttribute(string valuePath, object value) : base(valuePath)
        {
            Value = value;
        }
		
        /// <summary>
        /// The value to be used in determining if the condition is true.
        /// </summary>
        public object Value { get; set; }
    }
}
