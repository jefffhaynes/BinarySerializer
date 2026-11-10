using System;
using System.Diagnostics.CodeAnalysis;

namespace BinarySerialization
{
    public interface ISubtypeFactory
    {
        bool TryGetKey(Type valueType, out object key);
        bool TryGetType(object key, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out Type type);
    }
}