using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies the alignment of a member or object subgraph.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FieldAlignmentAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        private readonly int _alignment;

        /// <summary>
        ///     Initializes a new instance of the FieldAlignment class with a constant alignment.
        /// </summary>
        /// <param name="alignment">The fixed alignment of the decorated member.</param>
        /// <param name="mode">An optional parameter specifying the alignment mode.</param>
        public FieldAlignmentAttribute(int alignment, FieldAlignmentMode mode = FieldAlignmentMode.LeftAndRight)
        {
            _alignment = alignment;
            Mode = mode;
        }

        /// <summary>
        ///     Initializes a new instance of the FieldAlignment class with a path pointing to a binding source member.
        ///     <param name="path">A path to the source member.</param>
        ///     <param name="mode">An optional parameter specifying the alignment mode.</param>
        /// </summary>
        public FieldAlignmentAttribute(string path, FieldAlignmentMode mode = FieldAlignmentMode.LeftAndRight)
            : base(path)
        {
            Mode = mode;
        }

        internal FieldAlignmentMode Mode { get; }

        /// <summary>
        ///     Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return _alignment;
        }
    }
}