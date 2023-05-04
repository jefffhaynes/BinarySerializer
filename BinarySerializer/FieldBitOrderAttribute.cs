using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies the bit allocation order of a member or object sub-graph.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FieldBitOrderAttribute : FieldBindingBaseAttribute, IBitOrderAttribute, IConstAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the FieldBitOrder class with a constant ordering.
        /// </summary>
        /// <param name="bitOrder">The bit order of the decorated member.</param>
        public FieldBitOrderAttribute(BitOrder bitOrder)
        {
            BitOrder = bitOrder;
        }

        /// <summary>
        ///     Initializes a new instance of the FieldBitOrder class with a path pointing to a binding source member.
        ///     <param name="path">A path to the source member.</param>
        /// </summary>
        public FieldBitOrderAttribute(string path) : base(path)
        {
        }

        /// <summary>
        ///     Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return BitOrder;
        }

        /// <summary>
        ///     The order in which bits are allocated for this member.
        /// </summary>
        public BitOrder BitOrder { get; }
    }
}
