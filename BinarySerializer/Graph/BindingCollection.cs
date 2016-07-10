using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph
{
    internal class BindingCollection : ReadOnlyCollection<Binding>, IBinding
    {
        public BindingCollection(IEnumerable<Binding> bindings) : base(bindings.ToList())
        {
            if(this.Count(binding => IsConst) > 1)
                throw new InvalidOperationException("A field may not specify more than one constant binding.");
        }

        public bool IsConst => this[0].IsConst;

        public object ConstValue => this[0].ConstValue;

        public object GetValue(ValueNode target)
        {
            var value = this[0].GetValue(target);

            // handle multiple bindings (probably unusual)
            if (Count > 1)
            {
                if(AdditionalBindings.Any(binding => !binding.GetValue(target).Equals(value)))
                    throw new InvalidOperationException("Multiple source fields bound to the same target must yield the same value.");
            }

            return value;
        }

        public void Bind(ValueNode target, Func<object> callback)
        {
            foreach (var binding in this)
            {
                if(binding.BindingMode != BindingMode.OneWay)
                    binding.Bind(target, callback);
            }
        }

        private IEnumerable<Binding> AdditionalBindings
            => this.Skip(1).Where(binding => binding.BindingMode != BindingMode.OneWayToSource);
    }
}
