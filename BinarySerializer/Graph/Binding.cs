using System;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph
{
    internal class Binding : IBinding
    {
        private readonly object _constValue;

        public Binding(FieldBindingBaseAttribute attribute, int level)
        {
            Path = attribute.Path;
            BindingMode = attribute.BindingMode;

            if (attribute is IConstAttribute constAttribute && Path == null)
            {
                IsConst = true;
                _constValue = constAttribute.GetConstValue();
            }

            if (attribute.ConverterType != null)
            {
                ValueConverter = Activator.CreateInstance(attribute.ConverterType) as IValueConverter;

                if (ValueConverter == null)
                {
                    var message = $"{attribute.ConverterType} does not implement IValueConverter.";
                    throw new InvalidOperationException(message);
                }

                ConverterParameter = attribute.ConverterParameter;
            }

            RelativeSourceMode = attribute.RelativeSourceMode;

            Level = level;

            SupportsDeferredEvaluation = attribute.SupportsDeferredBinding;
        }

        public string Path { get; }
        public BindingMode BindingMode { get; }
        public IValueConverter ValueConverter { get; }
        public object ConverterParameter { get; }
        public RelativeSourceMode RelativeSourceMode { get; }
        public int Level { get; set; }
        public bool SupportsDeferredEvaluation { get; }

        public bool IsConst { get; }

        public object ConstValue
        {
            get
            {
                if (!IsConst)
                {
                    throw new InvalidOperationException("Not const.");
                }

                return _constValue;
            }
        }
        
        public object GetValue(ValueNode target)
        {
            if (IsConst)
            {
                return _constValue;
            }

            if (BindingMode == BindingMode.OneWayToSource)
            {
                return null;
            }

            var source = GetSource(target);

            CheckSource(source);

            return ValueConverter == null
                ? source.Value
                : Convert(source.Value, target.CreateLazySerializationContext());
        }

        public object GetBoundValue(ValueNode target)
        {
            if (IsConst)
            {
                return _constValue;
            }

            var source = GetSource(target);

            CheckSource(source);

            return ValueConverter == null
                ? source.BoundValue
                : Convert(source.BoundValue, target.CreateLazySerializationContext());
        }

        public void Bind(ValueNode target, Func<object> callback)
        {
            if (IsConst)
            {
                return;
            }

            var source = GetSource(target);

            var finalCallback = ValueConverter == null
                ? callback
                : () => ConvertBack(callback(), target.CreateLazySerializationContext());

            source.Bindings.Add(finalCallback);
        }

        public TNode GetSource<TNode>(TNode target) where TNode : Node<TNode>
        {
            if (IsConst)
            {
                throw new InvalidOperationException("Binding is constant.");
            }

            var relativeSource = GetRelativeSource(target);
            return relativeSource.GetChild(Path);
        }

        private TNode GetRelativeSource<TNode>(TNode target) where TNode : Node<TNode>
        {
            TNode source = null;

            switch (RelativeSourceMode)
            {
                case RelativeSourceMode.Self:
                    source = target.Parent;
                    break;
                case RelativeSourceMode.FindAncestor:
                case RelativeSourceMode.SerializationContext:
                    source = FindAncestor(target);
                    break;
            }

            return source;
        }

        private TNode FindAncestor<TNode>(TNode target) where TNode : Node<TNode>
        {
            var level = 1;
            var parent = target.Parent;
            while (parent != null)
            {
                if (level == Level)
                {
                    return parent;
                }

                if (parent.Parent == null && RelativeSourceMode == RelativeSourceMode.SerializationContext)
                {
                    return parent;
                }

                parent = parent.Parent;

                if (!(parent is RootValueNode))
                {
                    level++;
                }
            }

            return null;
        }

        private object Convert(object value, BinarySerializationContext context)
        {
            return ValueConverter.Convert(value, ConverterParameter, context);
        }

        private object ConvertBack(object value, BinarySerializationContext context)
        {
            return ValueConverter.ConvertBack(value, ConverterParameter, context);
        }
        
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void CheckSource(ValueNode source)
        {
            if (!source.Visited && !SupportsDeferredEvaluation && !source.TypeNode.IsIgnored)
            {
                throw new InvalidOperationException(
                    "This attribute does not support forward binding.  Consider specifying a BindingMode of OneWayToSource.");
            }
        }
    }
}