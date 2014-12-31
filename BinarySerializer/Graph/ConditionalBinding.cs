using System;

namespace BinarySerialization.Graph
{
    internal class ConditionalBinding : Binding
    {
        private readonly object _conditionalValue;

        public ConditionalBinding(Node node, SerializeWhenAttribute attribute, Func<object> targetEvaluator ) : base(node, attribute, targetEvaluator)
        {
            _conditionalValue = attribute.Value;
        }

        public bool Value
        {
            get { return GetValue().Equals(_conditionalValue); }
        }

        public bool BoundValue
        {
            get { return GetBoundValue().Equals(_conditionalValue); }
        }
    }
}
