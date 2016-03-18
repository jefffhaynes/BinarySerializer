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
        /// Initializes a new instance of the FieldOffset class.
        /// </summary>
        public FieldOffsetAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FieldOffset class with a fixed offset.
        /// </summary>
        /// <param name="constOffset"></param>
        public FieldOffsetAttribute(ulong constOffset)
        {
            ConstOffset = constOffset;
        }

        /// <summary>
        /// Initializes a new instance of the FieldOffset class with a path pointing to a source binding member.
        /// </summary>
        /// <param name="offsetPath">A path to the source member.</param>
        public FieldOffsetAttribute(string offsetPath) : base(offsetPath)
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
