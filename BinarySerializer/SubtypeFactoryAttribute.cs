using System;
using System.Diagnostics.CodeAnalysis;

namespace BinarySerialization
{
    /// <summary>
    /// Used to denote the type of a subtype factory object that implements ISubtypeFactory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SubtypeFactoryAttribute : SubtypeFactoryBaseAttribute
    {
        public SubtypeFactoryAttribute(string path, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type factoryType) : base(path, factoryType)
        {
        }
    }
}