using System;

namespace BinarySerialization
{
    /// <summary>
    /// Represents an binding exception.
    /// </summary>
    public class BindingException : Exception
    {
        internal BindingException()
        {
        }

        internal BindingException(string message)
            : base(message)
        {
        }

        internal BindingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
