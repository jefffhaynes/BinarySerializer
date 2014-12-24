using System;

namespace BinarySerialization
{
    internal class ObjectBinding : Binding
    {
        public ObjectBinding(Node targetNode, IBindableFieldAttribute attribute, Func<object> targetEvaluator = null) 
            : base(targetNode, attribute, targetEvaluator)
        {
        }

        public object Value
        {
            get { return GetValue(); }
        }

        public object BoundValue
        {
            get { return GetBoundValue(); }
        }
    }
}
