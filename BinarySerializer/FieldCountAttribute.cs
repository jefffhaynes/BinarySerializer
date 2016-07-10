using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies the number of items in a collection or array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FieldCountAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the FieldCount class.
        /// </summary>
        public FieldCountAttribute()
        {
            ConstCount = ulong.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the FieldCount class with a fixed item count.
        /// </summary>
        /// <param name="constCount">Used to specify the number of items in a fixed-size collection or array.</param>
        public FieldCountAttribute(ulong constCount)
        {
            ConstCount = constCount;
        }

        /// <summary>
        /// Initializes a new instance of the FieldCount class with a path pointing to a source binding member.
        /// </summary>
        /// <param name="countPath">A path to the source member.</param>
        public FieldCountAttribute(string countPath) : base(countPath)
        {
        }

        /// <summary>
        /// The number of items in the decorated collection or array for fixed-size collections or arrays.
        /// </summary>
        public ulong ConstCount { get; set; }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstCount;
        }
    }
}
