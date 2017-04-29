using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies an absolute offset of a member in the stream.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldOffsetAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the FieldOffset attribute with a fixed offset.
        /// </summary>
        /// <param name="offset"></param>
        public FieldOffsetAttribute(ulong offset)
        {
            ConstOffset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the FieldOffset attribute with a path pointing to a source binding member.
        /// </summary>
        /// <param name="path">A path to the source member.</param>
        public FieldOffsetAttribute(string path) : base(path)
        {
        }

        /// <summary>
        /// Used to specify fixed member offsets.
        /// </summary>
        public ulong ConstOffset { get; set; }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstOffset;
        }
    }
}
