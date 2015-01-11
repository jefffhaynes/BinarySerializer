using System;
using System.Linq;

namespace BinarySerialization.Graph
{
    internal class Binding
    {
        private const char PathSeparator = '.';

        protected Binding(BindingInfo bindingInfo, int level)
        {
            Path = bindingInfo.Path;

            if (bindingInfo.ConverterType != null)
            {
                ValueConverter = Activator.CreateInstance(bindingInfo.ConverterType) as IValueConverter;

                if (ValueConverter == null)
                {
                    var message = string.Format("{0} does not implement IValueConverter.", bindingInfo.ConverterType);
                    throw new InvalidOperationException(message);
                }

                ConverterParameter = bindingInfo.ConverterParameter;
            }

            Mode = bindingInfo.Mode;

            Level = level;
        }

        public string Path { get; private set; }

        public IValueConverter ValueConverter { get; private set; }

        public object ConverterParameter { get; private set; }

        public RelativeSourceMode Mode { get; private set; }

        public int Level { get; set; }

        public Node GetSource(Node target)
        {
            var relativeSource = GetRelativeSource(target);

            string[] memberNames = Path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new BindingException("Path cannot be empty.");

            Node relativeSourceChild = relativeSource;
            foreach (string name in memberNames)
            {
                relativeSourceChild = relativeSourceChild.Children.SingleOrDefault(c => c.Name == name);

                if (relativeSourceChild == null)
                    throw new BindingException(string.Format("No field found at '{0}'.", Path));
            }

            return relativeSourceChild;
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

        public void Bind(Node target, Func<object> callback)
        {
            var source = GetSource(target);
            source.TargetBindings.Add(callback);
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

        //public object Convert(object value)
        //{
        //    if (_valueConverter == null)
        //        return value;

        //    return _valueConverter.Convert(value, _converterParameter, _targetNode.CreateSerializationContext());
        //}

        //public object ConvertBack(object value)
        //{
        //    if (_valueConverter == null)
        //        return value;

        //    return _valueConverter.ConvertBack(value, _converterParameter, _targetNode.CreateSerializationContext());
        //}
    }
}
