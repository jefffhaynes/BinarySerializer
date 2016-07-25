namespace BinarySerialization
{
    /// <summary>
    /// Specifies the alignment of a member or object subgraph.
    /// </summary>
    public sealed class FieldAlignmentAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        private readonly int _alignment;

        /// <summary>
        /// Initializes a new instance of the FieldAlignment class with a constant alignment.
        /// </summary>
        /// <param name="alignment">The fixed alignment of the decorated member.</param>
        public FieldAlignmentAttribute(int alignment)
        {
            _alignment = alignment;
        }

        /// <summary>
        /// Initializes a new instance of the FieldAlignment class with a path pointing to a binding source member.
        /// <param name="alignmentPath">A path to the source member.</param> 
        /// </summary>
        public FieldAlignmentAttribute(string alignmentPath) : base(alignmentPath)
        {
        }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return _alignment;
        }
    }
}
