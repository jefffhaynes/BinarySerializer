namespace BinarySerialization;

/// <summary>
///     Based on Making Reflection Fly And Exploring Delegates by Jon Skeet
/// </summary>
internal static class MagicMethods
{
    public static Func<object, object> MagicFunc(Type targetType, MethodInfo method)
    {
        // First fetch the generic form
        var genericHelper = typeof(MagicMethods).GetMethod(nameof(MagicFuncHelper),
            BindingFlags.Static | BindingFlags.NonPublic);

        // Now supply the type arguments
        var constructedHelper = genericHelper.MakeGenericMethod
            (targetType, method.ReturnType);

        // Now call it. The null argument is because it’s a static method.
        var ret = constructedHelper.Invoke(null, new object[] { method });

        // Cast the result to the right kind of delegate and return it
        return (Func<object, object>)ret;
    }

    public static Action<object, object> MagicAction(Type targetType, MethodInfo method)
    {
        // First fetch the generic form
        var genericHelper = typeof(MagicMethods).GetMethod(nameof(MagicActionHelper),
            BindingFlags.Static | BindingFlags.NonPublic);

        // Now supply the type arguments
        var constructedHelper = genericHelper.MakeGenericMethod
            (targetType, method.GetParameters().Single().ParameterType);

        // Now call it. The null argument is because it’s a static method.
        var ret = constructedHelper.Invoke(null, new object[] { method });

        // Cast the result to the right kind of delegate and return it
        return (Action<object, object>)ret;
    }

    // ReSharper disable once UnusedMember.Local
    private static Func<object, object> MagicFuncHelper<TTarget, TReturn>(MethodInfo method)
    {
        // Convert the slow MethodInfo into a fast, strongly typed, open delegate
        var func = (Func<TTarget, TReturn>)method.CreateDelegate(typeof(Func<TTarget, TReturn>));

        // Now create a more weakly typed delegate which will call the strongly typed one
        object Func(object target)
        {
            return func((TTarget)target);
        }

        return Func;
    }

    // ReSharper disable once UnusedMember.Local
    private static Action<object, object> MagicActionHelper<TTarget, TValue>(MethodInfo method)
    {
        // Convert the slow MethodInfo into a fast, strongly typed, open delegate
        var action = (Action<TTarget, TValue>)method.CreateDelegate(typeof(Action<TTarget, TValue>));

        // Now create a more weakly typed delegate which will call the strongly typed one
        void Func(object target, object value)
        {
            if (value != null)
            {
                action((TTarget)target, (TValue)value);
            }
        }

        return Func;
    }
}
