using System.Collections.Generic;
using System.Linq;
using BinarySerialization;

namespace BinarySerialization
{
    internal class ConditionalAttributeEvaluator
    {
        private readonly Dictionary<Node, object> _sourceValue = new Dictionary<Node, object>();

        public ConditionalAttributeEvaluator(Node node, IEnumerable<SerializeWhenAttribute> attributes)
        {
            _sourceValue = attributes.ToDictionary(attribute => node.GetBindingSource(attribute.Binding),
                attribute => attribute.Value);
        }

        public bool Value
        {
            get { return _sourceValue.Any(sourceValue => sourceValue.Value.Equals(sourceValue.Key.Value)); }
        }

        public bool BoundValue
        {
            get { return _sourceValue.Any(sourceValue => sourceValue.Value.Equals(sourceValue.Key.BoundValue)); }
        }
    }
}
