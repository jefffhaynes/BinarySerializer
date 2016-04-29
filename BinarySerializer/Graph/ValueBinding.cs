namespace BinarySerialization.Graph
{
    internal class ValueBinding : Binding
    {
        private readonly FieldValueAttribute _attribute;

        public ValueBinding(FieldValueAttribute attribute, int level) : base(attribute, level)
        {
            _attribute = attribute;
        }
    }
}
