using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to specify the length of fixed-length collection items such as byte arrays and fixed-length strings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ItemLengthAttribute : FieldBindingBaseAttribute, ILengthAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the ItemLength class with a constant length.
        /// </summary>
        /// <param name="constLength">The fixed-size length of the decorated member.</param>
        public ItemLengthAttribute(ulong constLength)
        {
            ConstLength = constLength;
        }

        /// <summary>
        /// Initializes a new instance of the ItemLength class with a path pointing to a binding source member.
        /// <param name="lengthPath">A path to the source member.</param> 
        /// </summary>
        public ItemLengthAttribute(string lengthPath)
            : base(lengthPath)
        {
        }

        /// <summary>
        /// Used to specify a constant field length.  This will be used if no binding is specified.
        /// </summary>
        public ulong ConstLength { get; set; }

        public object GetConstValue()
        {
            return ConstLength;
        }
    }
}
