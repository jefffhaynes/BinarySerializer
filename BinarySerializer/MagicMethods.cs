using System;
using System.Reflection;

namespace BinarySerialization
{
    internal static class MagicMethods
    {
        /// <summary>
        /// Based on Making Reflection Fly And Exploring Delegates by Jon Skeet
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Func<object, object> MagicMethod(Type targetType, MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = typeof(MagicMethods).GetMethod("MagicMethodHelper",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
                (targetType, method.ReturnType);

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<object, object>)ret;
        }

        // ReSharper disable UnusedMember.Local
        private static Func<object, object> MagicMethodHelper<TTarget, TReturn>(MethodInfo method)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TReturn> func = (Func<TTarget, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TTarget, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<object, object> ret = target => func((TTarget)target);
            return ret;
        }
        // ReSharper restore UnusedMember.Local

        public static Func<object, object, object> MagicMethod<T>(MethodInfo method) where T : class
        {
            // First fetch the generic form
            MethodInfo genericHelper = typeof(MagicMethods).GetMethod("MagicMethodHelper2",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
                (typeof(T), method.GetParameters()[0].ParameterType, method.ReturnType);

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<object, object, object>)ret;
        }

        // ReSharper disable UnusedMember.Local
        private static Func<TTarget, object, object> MagicMethodHelper2<TTarget, TParam, TReturn>(MethodInfo method)
            where TTarget : class
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TParam, TReturn> func = (Func<TTarget, TParam, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TTarget, TParam, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<object, object, object> ret = (target, param) => func((TTarget)target, (TParam)param);
            return ret;
        }
        // ReSharper restore UnusedMember.Local
    }
}
