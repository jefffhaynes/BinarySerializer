using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Used to control conditional serialization of members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class SerializeWhenNotAttribute : ConditionalAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SerializeWhenNotAttribute" />.
        /// </summary>
        /// <param name="valuePath">The path to the binding source.</param>
        /// <param name="value">The value to be used in determining if the condition is false.</param>
        public SerializeWhenNotAttribute(string valuePath, object value) : base(valuePath, value)
        {
        }
    }
}