using System;

namespace BinarySerialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SubtypeFactoryAttribute : SubtypeFactoryBaseAttribute
    {
        public SubtypeFactoryAttribute(string path, Type factoryType) : base(path, factoryType)
        {
        }
    }
}
