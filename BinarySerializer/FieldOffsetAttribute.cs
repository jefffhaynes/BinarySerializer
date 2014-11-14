using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies an absolute offset of a member in the stream.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FieldOffsetAttribute : FieldBindingBaseAttribute
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
        public FieldOffsetAttribute(int constOffset)
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
        public long ConstOffset { get; set; }

        internal override bool IsConstSupported
        {
            get { return true; }
        }
    }
}
