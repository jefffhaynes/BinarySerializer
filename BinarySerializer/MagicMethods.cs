﻿using System;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    /// <summary>
    /// Based on Making Reflection Fly And Exploring Delegates by Jon Skeet
    /// </summary>
    internal static class MagicMethods
    {
        public static Func<object, object> MagicFunc(Type targetType, MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = typeof(MagicMethods).GetMethod("MagicFuncHelper",
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
        private static Func<object, object> MagicFuncHelper<TTarget, TReturn>(MethodInfo method)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TReturn> func = (Func<TTarget, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TTarget, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<object, object> ret = target => func((TTarget)target);
            return ret;
        }
        // ReSharper restore UnusedMember.Local

        public static Action<object, object> MagicAction(Type targetType, MethodInfo method)
        {
            // First fetch the generic form
            MethodInfo genericHelper = typeof(MagicMethods).GetMethod("MagicActionHelper",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
                (targetType, method.GetParameters().Single().ParameterType);

            // Now call it. The null argument is because it’s a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Action<object, object>)ret;
        }

        // ReSharper disable UnusedMember.Local
        private static Action<object, object> MagicActionHelper<TTarget, TValue>(MethodInfo method)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Action<TTarget, TValue> action = (Action<TTarget, TValue>)Delegate.CreateDelegate
                (typeof(Action<TTarget, TValue>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Action<object, object> ret = (target, value) =>
            {
                if (value != null)
                    action((TTarget) target, (TValue) value);
            };
            return ret;
        }
        // ReSharper restore UnusedMember.Local
    }
}
