using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal static class ReflectionExtensionMethods
    {
        public static T GetAttribute<T>(this MemberInfo type)
        {
            //return (T) Convert.ChangeType(PlayerStats[type], typeof(T));
#if PORTABLE328
            return (T)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
#else
            return (T)type.GetCustomAttributes(typeof(T),true).SingleOrDefault().ConvertTo(typeof(T));
 #endif
                }
    }
}
