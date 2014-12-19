using System;

namespace GraphGen
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
