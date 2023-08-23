using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Instructs <see cref="BinarySerializer" /> not to serialize or deserialize the specified public field or property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class IgnoreMemberAttribute : Attribute
    {
        public IgnoreMemberAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}