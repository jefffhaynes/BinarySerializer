namespace BinarySerialization.Graph
{
    public class ConditionalBinding : Binding
    {
        public ConditionalBinding(SerializeWhenAttribute attribute, int level)
            : base(attribute, level)
        {
            ConditionalValue = attribute.Value;
        }

        public object ConditionalValue { get; private set; }
    }
}
