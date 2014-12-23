using System;

namespace BinarySerialization
{
    internal class Binding
    {
        public Binding(Func<object> targetValueGetter)
        {
            TargetValueGetter = targetValueGetter;
        }

        public Func<object> TargetValueGetter { get; private set; }
    }
}
