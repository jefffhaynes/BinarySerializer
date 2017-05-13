using System;

namespace BinarySerialization.Graph
{
    internal class ConditionalBinding : Binding
    {
        private readonly Type _conditionalValueType;

        public ConditionalBinding(ConditionalAttribute attribute, int level)
            : base(attribute, level)
        {
            ConditionalValue = attribute.Value;

            if (ConditionalValue != null)
            {
                _conditionalValueType = ConditionalValue.GetType();
            }
        }

        public object ConditionalValue { get; }

        public bool IsSatisfiedBy(object value)
        {
            if (ConditionalValue == null && value == null)
            {
                return true;
            }

            if (ConditionalValue == null || value == null)
            {
                return false;
            }

            var convertedValue = value.ConvertTo(_conditionalValueType);

            return convertedValue.Equals(ConditionalValue);
        }
    }
}