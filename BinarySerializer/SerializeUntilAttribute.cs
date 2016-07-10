using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies an explicit list termination condition when the condition is defined external to the items in
    /// the collection (e.g. a null-terminated list).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializeUntilAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializeUntilAttribute"/> class with a terminating constValue.
        /// </summary>
        /// <param name="constValue"></param>
        public SerializeUntilAttribute(object constValue)
        {
            ConstValue = constValue;
        }
		
        /// <summary>
        /// The terminating constValue.
        /// </summary>
        public object ConstValue { get; set; }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstValue;
        }
    }
}
