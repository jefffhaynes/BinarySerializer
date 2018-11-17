using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Instructs <see cref="BinarySerializer" /> not to serialize or deserialize the public field or property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}