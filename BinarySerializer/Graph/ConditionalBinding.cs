namespace BinarySerialization.Graph
{
    internal class ConditionalBinding : Binding
    {
        public ConditionalBinding(SerializeWhenAttribute attribute, int level)
            : base(attribute, level)
        {
            ConditionalValue = attribute.Value;
        }

        public object ConditionalValue { get; private set; }
    }
}
