namespace BinarySerialization.Graph
{
    internal class ValueBinding : Binding
    {
        private readonly FieldValueAttributeBase _attribute;

        public ValueBinding(FieldValueAttributeBase attribute, int level) : base(attribute, level)
        {
            _attribute = attribute;
        }
    }
}
