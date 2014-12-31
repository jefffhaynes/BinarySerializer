using System;

namespace BinarySerialization.Graph
{
    internal abstract class Binding
    {
        private readonly IValueConverter _valueConverter;
        private readonly object _converterParameter;
        private readonly Func<object> _targetEvaluator;
        private readonly Node _targetNode;
        private readonly BindingInfo _bindingInfo;

        protected Binding(Node targetNode, IBindableFieldAttribute attribute, Func<object> targetEvaluator = null)
        {
            if (string.IsNullOrEmpty(attribute.Path))
                return;

            _targetNode = targetNode;
            _bindingInfo = attribute.Binding;

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

        public Node GetSource()
        {
            if (_bindingInfo == null)
                return null;

            return _targetNode.GetBindingSource(_bindingInfo);
        }

        public void Bind()
        {
            var source = GetSource();

            if (source != null && _targetEvaluator != null)
            {
                if (!source.Bindings.Contains(this))
                    source.Bindings.Add(this);
            }
        }

        public void Unbind()
        {
            var source = GetSource();

            if (source != null)
            {
                source.Bindings.Remove(this);
            }
        }

        protected object GetValue()
        {
            return Convert(GetSource().Value);
        }

        protected object GetBoundValue()
        {
            return Convert(GetSource().BoundValue);
        }

        public object GetTargetValue()
        {
            return ConvertBack(_targetEvaluator());
        }

        public object Convert(object value)
        {
            if (_valueConverter == null)
                return value;

            return _valueConverter.Convert(value, _converterParameter, _targetNode.CreateSerializationContext());
        }

        public object ConvertBack(object value)
        {
            if (_valueConverter == null)
                return value;

            return _valueConverter.ConvertBack(value, _converterParameter, _targetNode.CreateSerializationContext());
        }
    }
}
