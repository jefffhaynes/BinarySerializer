using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal static class ReflectionExtensionMethods
    {
        public static T GetAttribute<T>(this MemberInfo type) where T : class
        {
            return type.GetCustomAttributes(typeof(T), true).SingleOrDefault() as T;
        }
    }
}
