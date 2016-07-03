using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal static class ReflectionExtensionMethods
    {
        public static T GetAttribute<T>(this MemberInfo type)
        {
            //TODO: VERIFY FOR DOTNETCORE
#if !PORTABLE328
            return (T)(object)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
#else
            return (T)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();

#endif
        }
    }
}
