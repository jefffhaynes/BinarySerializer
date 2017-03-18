using System;
using System.Reflection;

namespace BinarySerialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class SubtypeFactoryBaseAttribute : FieldBindingBaseAttribute
    {
        protected SubtypeFactoryBaseAttribute(string path, Type factoryType) : base(path)
        {
            if (!typeof(ISubtypeFactory).IsAssignableFrom(factoryType))
            {
                throw new ArgumentException("Factory type must implement ISubtypeFactory", nameof(factoryType));
            }

            FactoryType = factoryType;
        }

        public Type FactoryType { get; }
    }
}
