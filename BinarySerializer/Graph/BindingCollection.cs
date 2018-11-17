using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph
{
    public class BindingCollection : ReadOnlyCollection<Binding>, IBinding
    {
        public BindingCollection(IEnumerable<Binding> bindings) : base(bindings.ToList())
        {
            if (this.Count(binding => IsConst) > 1)
            {
                throw new InvalidOperationException("A field may not specify more than one constant binding.");
            }
        }

        private IEnumerable<Binding> AdditionalBindings
            => this.Skip(1).Where(binding => binding.BindingMode != BindingMode.OneWayToSource);

        public bool IsConst => this[0].IsConst;

        public object ConstValue => this[0].ConstValue;

        public object GetValue(ValueNode target)
        {
            return GetValue(binding => binding.GetValue(target));
        }

        public object GetBoundValue(ValueNode target)
        {
            return GetValue(binding => binding.GetBoundValue(target));
        }

        public void Bind(ValueNode target, Func<object> callback)
        {
            foreach (var binding in this)
            {
                if (binding.BindingMode != BindingMode.OneWay)
                {
                    binding.Bind(target, callback);
                }
            }
        }

        private object GetValue(Func<Binding, object> bindingFunction)
        {
            // get first value and use that to compare any others
            var value = bindingFunction(this[0]);

            // handle multiple bindings (probably not typical)
            if (Count > 1)
            {
                if (AdditionalBindings.Any(binding =>
                {
                    var otherValue = bindingFunction(binding);

                    if (value != null)
                    {
                        var convertedValue = Convert.ChangeType(otherValue, value.GetType(), null);

                        if (convertedValue == null)
                        {
                            return false;
                        }

                        return !convertedValue.Equals(value);
                    }

                    return otherValue == null;
                }))
                {
                    throw new InvalidOperationException(
                        "Multiple source fields bound to the same target must yield the same value.");
                }
            }

            return value;
        }
    }
}