using System;

namespace BinarySerialization
{
    internal abstract class AttributeEvaluator<TValue>
    {
        private readonly IValueConverter _valueConverter;
        private readonly object _converterParameter;

        protected AttributeEvaluator(Node node, IBindableFieldAttribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.Path))
                return;

            Source = node.GetBindingSource(attribute.Binding);

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
        }

        public abstract TValue Value { get; }

        public abstract TValue BoundValue { get; }

        public Node Source { get; private set; }

        protected object GetValue()
        {
            if (_valueConverter == null)
                return Source.Value;

            // TODO BS ctx
            return _valueConverter.ConvertBack(Source.Value, _converterParameter, null);
        }

        protected object GetBoundValue()
        {
            if (_valueConverter == null)
                return Source.BoundValue;

            // TODO BS ctx
            return _valueConverter.ConvertBack(Source.BoundValue, _converterParameter, null);
        }
    }
}
