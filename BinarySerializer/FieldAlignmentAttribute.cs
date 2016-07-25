namespace BinarySerialization
{
    public sealed class FieldAlignmentAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        private readonly int _alignment;

        public FieldAlignmentAttribute(int alignment)
        {
            _alignment = alignment;
        }

        public FieldAlignmentAttribute(string alignmentPath) : base(alignmentPath)
        {
        }

        public object GetConstValue()
        {
            return _alignment;
        }
    }
}
