using System;

namespace BinarySerialization
{
    [Flags]
    public enum SerializationOptions
    {
        None = 0,
        ThrowOnEndOfStream = 1
    }
}
