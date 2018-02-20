using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies the length of a member or object subgraph.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldBitLengthAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the FieldLength class with a constant length.
        /// </summary>
        /// <param name="length">The fixed-size length of the decorated member.</param>
        public FieldBitLengthAttribute(ulong length)
        {
            ConstLength = length;
        }

        /// <summary>
        ///     Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstLength;
        }

        /// <summary>
        ///     The number of items in the decorated member for fixed-sized members or object subgraphs.
        /// </summary>
        public ulong ConstLength { get; }
    }
}