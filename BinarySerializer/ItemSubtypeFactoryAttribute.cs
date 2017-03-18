using System;

namespace BinarySerialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ItemSubtypeFactoryAttribute : SubtypeFactoryBaseAttribute
    {
        public ItemSubtypeFactoryAttribute(string path, Type factoryType) : base(path, factoryType)
        {
        }
    }
}
