using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to denote the type of a subtype factory object that implements ISubtypeFactory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SubtypeFactoryAttribute : SubtypeFactoryBaseAttribute
    {
        public SubtypeFactoryAttribute(string path, Type factoryType) : base(path, factoryType)
        {
        }
    }
}