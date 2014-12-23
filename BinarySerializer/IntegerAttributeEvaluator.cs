using System;

namespace BinarySerialization
{
    internal class IntegerAttributeEvaluator : IAttributeEvaluator<ulong>
    {
        private readonly ulong? _constValue;
        private readonly Node _source;
 
        public IntegerAttributeEvaluator(Node node, IIntegerAttribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.Path))
            {
                _constValue = attribute.GetConstValue();
            }
            else
            {
                _source = node.GetBindingSource(attribute.Binding);
            }
        }

        public bool IsConst
        {
            get { return _constValue.HasValue; }
        }

        public ulong Value
        {
            get
            {
                return _constValue.HasValue ? _constValue.Value : Convert.ToUInt64(_source.Value);
            }
        }

        public ulong BoundValue
        {
            get
            {
                return _constValue.HasValue ? _constValue.Value : Convert.ToUInt64(_source.BoundValue);
            }
        }

        public Node Source { get { return _source; } }
    }
}
