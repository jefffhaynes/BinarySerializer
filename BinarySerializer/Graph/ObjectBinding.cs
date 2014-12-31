using System;

namespace BinarySerialization.Graph
{
    internal class ObjectBinding : Binding
    {
        private readonly object _constValue;

        public ObjectBinding(Node targetNode, IBindableFieldAttribute attribute, Func<object> targetEvaluator = null) 
            : base(targetNode, attribute, targetEvaluator)
        {
            var constAttribute = attribute as IConstAttribute;
            if (constAttribute != null)
            {
                _constValue = constAttribute.GetConstValue();
            }
        }

        public bool IsConst
        {
            get { return _constValue != null; }
        }

        public object Value
        {
            get { return IsConst ? _constValue : GetValue(); }
        }

        public object BoundValue
        {
            get { return IsConst ? _constValue : GetBoundValue(); }
        }
    }
}
