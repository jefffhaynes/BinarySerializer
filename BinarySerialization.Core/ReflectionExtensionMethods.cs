using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal static class ReflectionExtensionMethods
    {
        public static T GetAttribute<T>(this MemberInfo type)
        {
            return (T)(object)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
        }
    }
}
