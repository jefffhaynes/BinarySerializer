using System;

namespace BinarySerialization
{
    /// <summary>
    /// Instructs the <see cref="BinarySerializer.Serialize"/> method and the <see cref="BinarySerializer.Deserialize"/> 
    /// of the <see cref="BinarySerializer"/> not to serialize the public field or public read/write property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}
