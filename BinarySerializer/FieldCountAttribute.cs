using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies the number of items in a collection or array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FieldCountAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the FieldCount class with a fixed item count.
        /// </summary>
        /// <param name="count">Used to specify the number of items in a fixed-size collection or array.</param>
        public FieldCountAttribute(ulong count)
        {
            ConstCount = count;
        }

        /// <summary>
        ///     Initializes a new instance of the FieldCount class with a path pointing to a source binding member.
        /// </summary>
        /// <param name="path">A path to the source member.</param>
        public FieldCountAttribute(string path) : base(path)
        {
        }

        /// <summary>
        ///     The number of items in the decorated collection or array for fixed-size collections or arrays.
        /// </summary>
        public ulong ConstCount { get; set; }

        /// <summary>
        ///     Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstCount;
        }
    }
}