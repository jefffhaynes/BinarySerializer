using System;

namespace BinarySerialization
{
    internal class LazyEventArgs<TEventArgs> : EventArgs where TEventArgs : EventArgs
    {
        public LazyEventArgs(Lazy<TEventArgs> eventArgs)
        {
            EventArgs = eventArgs;
        }

        public Lazy<TEventArgs> EventArgs { get; private set; } 
    }
}
