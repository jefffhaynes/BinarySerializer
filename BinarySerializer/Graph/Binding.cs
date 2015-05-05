using System;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph
{
    internal class Binding
    {
        private readonly object _constValue;

        public Binding(IBindableFieldAttribute attribute, int level)
        {
            Path = attribute.Path;
            BindingMode = attribute.BindingMode;

            var constAttribute = attribute as IConstAttribute;
            if (constAttribute != null && Path == null)
            {
                IsConst = true;
                _constValue = constAttribute.GetConstValue();
            }

            if (attribute.ConverterType != null)
            {
                ValueConverter = Activator.CreateInstance(attribute.ConverterType) as IValueConverter;

                if (ValueConverter == null)
                {
                    var message = string.Format("{0} does not implement IValueConverter.", attribute.ConverterType);
                    throw new InvalidOperationException(message);
                }

                ConverterParameter = attribute.ConverterParameter;
            }

            RelativeSourceMode = attribute.RelativeSourceMode;

            Level = level;
        }

        public bool IsConst { get; private set; }

        public object ConstValue
        {
            get
            {
                if (!IsConst)
                    throw new InvalidOperationException("Not const.");

                return _constValue;
            }
        }

        public string Path { get; private set; }

        public BindingMode BindingMode { get; private set; }

        public IValueConverter ValueConverter { get; private set; }

        public object ConverterParameter { get; private set; }

        public RelativeSourceMode RelativeSourceMode { get; private set; }

        public int Level { get; set; }

        public object GetValue(ValueNode target)
        {
            if (IsConst)
                return _constValue;

            var source = GetSource(target);

            return ValueConverter == null ? source.Value : Convert(source.Value, target.CreateSerializationContext());
        }

        public object GetBoundValue(ValueNode target)
        {
            if (IsConst)
                return _constValue;

            var source = GetSource(target);

            return ValueConverter == null
                ? source.BoundValue
                : Convert(source.BoundValue, target.CreateSerializationContext());
        }

        public ValueNode GetSource(ValueNode target)
        {
            var relativeSource = (ValueNode) GetRelativeSource(target);
            return relativeSource.GetChild(Path);
        }

        private Node GetRelativeSource(Node target)
        {
            Node source = null;

            switch (RelativeSourceMode)
            {
                case RelativeSourceMode.Self:
                    source = target.Parent;
                    break;
                case RelativeSourceMode.FindAncestor:
                case RelativeSourceMode.SerializationContext:
                    source = FindAncestor(target);
                    break;
                case RelativeSourceMode.PreviousData:
                    throw new NotImplementedException();
            }

            return source;
        }

        private Node FindAncestor(Node target)
        {
            int level = 1;
            Node parent = target.Parent;
            while (parent != null)
            {
                if (level == Level)
                    return parent;

                if (parent.Parent == null && RelativeSourceMode == RelativeSourceMode.SerializationContext)
                    return parent;

                parent = parent.Parent;

                if (!(parent is ContextValueNode))
                    level++;
            }

            return null;
        }

        public void Bind(ValueNode target, Func<object> callback)
        {
            if (IsConst)
                return;

            var source = GetSource(target);

            Func<object> finalCallback = ValueConverter == null
                ? callback
                : () => ConvertBack(callback(), target.CreateSerializationContext());

            source.TargetBindings.Add(finalCallback);
        }

        private object Convert(object value, BinarySerializationContext context)
        {
            return ValueConverter.Convert(value, ConverterParameter, context);
        }

        private object ConvertBack(object value, BinarySerializationContext context)
        {
            return ValueConverter.ConvertBack(value, ConverterParameter, context);
        }
    }
}
