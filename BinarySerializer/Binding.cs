using System;

namespace BinarySerialization
{
    internal abstract class Binding
    {
        private readonly IValueConverter _valueConverter;
        private readonly object _converterParameter;
        private readonly Func<object> _targetEvaluator;

        protected Binding(Node targetNode, IBindableFieldAttribute attribute, Func<object> targetEvaluator = null)
        {
            if (string.IsNullOrEmpty(attribute.Path))
                return;

            Source = targetNode.GetBindingSource(attribute.Binding);

            if (Source != null && targetEvaluator != null)
            {
                Source.Bindings.Add(this);
            }

            if (attribute.ConverterType != null)
            {
                var valueConverter = Activator.CreateInstance(attribute.ConverterType) as IValueConverter;

                if (valueConverter == null)
                {
                    var message = string.Format("{0} does not implement IValueConverter.", attribute.ConverterType);
                    throw new InvalidOperationException(message);
                }

                _valueConverter = valueConverter;
                _converterParameter = attribute.ConverterParameter;
            }

            _targetEvaluator = targetEvaluator;
        }

        public Node Source { get; private set; }

        protected object GetValue()
        {
            return Convert(Source.Value);
        }

        protected object GetBoundValue()
        {
            return Convert(Source.BoundValue);
        }

        public object GetTargetValue()
        {
            return ConvertBack(_targetEvaluator());
        }

        public object Convert(object value)
        {
            if (_valueConverter == null)
                return value;

            return _valueConverter.Convert(value, _converterParameter, null);
        }

        public object ConvertBack(object value)
        {
            if (_valueConverter == null)
                return value;

            return _valueConverter.ConvertBack(value, _converterParameter, null);
        }
    }
}
