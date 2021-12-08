using System;

namespace BinarySerialization.Graph
{
    internal class ConditionalBinding : Binding
    {
        private readonly object _conditionalValue;
        private readonly Type _conditionalValueType;
        private readonly ComparisonOperator _comparisonOperator;

        public ConditionalBinding(SerializeWhenAttribute attribute, int level)
            : base(attribute, level)
        {
            _conditionalValue = attribute.Value;

            if (_conditionalValue != null)
            {
                _conditionalValueType = _conditionalValue.GetType();
            }

            _comparisonOperator = attribute.Operator;
        }

        public bool IsSatisfiedBy(object value)
        {
            switch (_comparisonOperator)
            {
                case ComparisonOperator.Equal:
                    return AreEqual(value);
                case ComparisonOperator.NotEqual:
                    return !AreEqual(value);
                case ComparisonOperator.LessThan:
                    return Compare(value, (lhs, rhs) => lhs < rhs);
                case ComparisonOperator.GreaterThan:
                    return Compare(value, (lhs, rhs) => lhs > rhs);
                case ComparisonOperator.LessThanOrEqual:
                    return Compare(value, (lhs, rhs) => lhs <= rhs);
                case ComparisonOperator.GreaterThanOrEqual:
                    return Compare(value, (lhs, rhs) => lhs >= rhs);
                default: throw new NotSupportedException();
            }
        }

        private bool AreEqual(object value)
        {
            if (_conditionalValue == null && value == null)
            {
                return true;
            }

            if (_conditionalValue == null || value == null)
            {
                return false;
            }

            var convertedValue = value.ConvertTo(_conditionalValueType);

            return convertedValue.Equals(_conditionalValue);
        }

        private bool Compare(object value, Func<double, double, bool> comparator)
        {
            if (value == null || _conditionalValue == null)
            {
                throw new InvalidOperationException("Unable to compare null values");
            }

            var convertedValue = value.ConvertTo(_conditionalValueType);

            var lhs = Convert.ToDouble(convertedValue);
            var rhs = Convert.ToDouble(_conditionalValue);

            return comparator(lhs, rhs);
        }
    }
}