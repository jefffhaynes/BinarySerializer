using System;

namespace BinarySerialization
{
    internal class IntegerAttributeEvaluator : AttributeEvaluator<ulong>
    {
        private readonly ulong? _constValue;
 
        public IntegerAttributeEvaluator(Node node, IIntegerAttribute attribute) : base(node, attribute)
        {
            if (string.IsNullOrEmpty(attribute.Path))
            {
                _constValue = attribute.GetConstValue();
            }
        }

        public bool IsConst
        {
            get { return _constValue.HasValue; }
        }

        public override ulong Value
        {
            get
            {
                return _constValue.HasValue ? _constValue.Value : Convert.ToUInt64(GetValue());
            }
        }

        public override ulong BoundValue
        {
            get
            {
                return _constValue.HasValue ? _constValue.Value : Convert.ToUInt64(GetBoundValue());
            }
        }
    }
}
