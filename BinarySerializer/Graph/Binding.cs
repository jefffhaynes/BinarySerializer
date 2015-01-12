using System;
using System.Linq;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph
{
    internal class Binding
    {
        private const char PathSeparator = '.';

        private readonly object _constValue;

        public Binding(IBindableFieldAttribute attribute, int level)
        {
            Path = attribute.Path;

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

            Mode = attribute.Mode;

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

        public IValueConverter ValueConverter { get; private set; }

        public object ConverterParameter { get; private set; }

        public RelativeSourceMode Mode { get; private set; }

        public int Level { get; set; }

        public object GetValue(ValueNode target)
        {
            if (IsConst)
                return _constValue;

            var source = GetSource(target);
            return Convert(source.Value);
        }

        public object GetBoundValue(ValueNode target)
        {
            if (IsConst)
                return _constValue;

            var source = GetSource(target);

            var bindingSource = source as IBindingSource;

            if(bindingSource == null)
                throw new InvalidOperationException("Not a bindable source.");

            return Convert(bindingSource.BoundValue);
        }

        public ValueNode GetSource(ValueNode target)
        {
            var relativeSource = GetRelativeSource(target);

            string[] memberNames = Path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new BindingException("Path cannot be empty.");

            var relativeSourceChild = relativeSource;
            foreach (string name in memberNames)
            {
                relativeSourceChild = relativeSourceChild.Children.SingleOrDefault(c => c.Name == name);

                if (relativeSourceChild == null)
                    throw new BindingException(string.Format("No field found at '{0}'.", Path));
            }

            return (ValueNode)relativeSourceChild;
        }

        private Node GetRelativeSource(Node target)
        {
            Node source = null;

            switch (Mode)
            {
                case RelativeSourceMode.Self:
                    source = target.Parent;
                    break;
                case RelativeSourceMode.FindAncestor:
                    source = FindAncestor(target);
                    break;
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

                parent = parent.Parent;
                level++;
            }

            return null;
        }

        public void Bind(ValueNode target, Func<object> callback)
        {
            if (IsConst)
                return;

            var source = GetSource(target);
            source.TargetBindings.Add(() => ConvertBack(callback()));
        }

        //protected object GetValue()
        //{
        //    return Convert(GetSource().Value);
        //}

        //protected object GetBoundValue()
        //{
        //    return Convert(GetSource().BoundValue);
        //}

        //public object GetTargetValue()
        //{
        //    return ConvertBack(_targetEvaluator());
        //}

        public object Convert(object value)
        {
            if (ValueConverter == null)
                return value;

            //return ValueConverter.Convert(value, _converterParameter, _targetNode.CreateSerializationContext());
            return ValueConverter.Convert(value, ConverterParameter, null);
        }

        public object ConvertBack(object value)
        {
            if (ValueConverter == null)
                return value;

            //return ValueConverter.ConvertBack(value, _converterParameter, _targetNode.CreateSerializationContext());
            return ValueConverter.ConvertBack(value, ConverterParameter, null);
        }
    }
}
