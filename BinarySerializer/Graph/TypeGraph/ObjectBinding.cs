using System;

namespace BinarySerialization.TypeGraph
{
    internal class ObjectBinding : Binding
    {
        private readonly object _constValue;

        public ObjectBinding(Node targetNode, BindingInfo binding) 
            : base(targetNode, binding)
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
