﻿namespace BinarySerialization;

internal static class TypeExtensions
{
    internal static readonly Dictionary<Type, Func<object, object>> TypeConverters =
        new Dictionary<Type, Func<object, object>>
        {
                {typeof(char), o => Convert.ToChar(o)},
                {typeof(byte), o => Convert.ToByte(o)},
                {typeof(sbyte), o => Convert.ToSByte(o)},
                {typeof(bool), o => Convert.ToBoolean(o)},
                {typeof(short), o => Convert.ToInt16(o)},
                {typeof(int), o => Convert.ToInt32(o)},
                {typeof(long), o => Convert.ToInt64(o)},
                {typeof(ushort), o => Convert.ToUInt16(o)},
                {typeof(uint), o => Convert.ToUInt32(o)},
                {typeof(ulong), o => Convert.ToUInt64(o)},
                {typeof(float), o => Convert.ToSingle(o)},
                {typeof(double), o => Convert.ToDouble(o)},
                {typeof(string), Convert.ToString}
        };

    public static object ConvertTo(this object value, Type type)
    {
        if (value == null)
        {
            return null;
        }

        var valueType = value.GetType();

        if (valueType == type)
        {
            return value;
        }

        /* Special handling for strings */
        if (valueType == typeof(string) && type.GetTypeInfo().IsPrimitive)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                value = 0;
            }
        }

        if (TypeConverters.TryGetValue(type, out var converter))
        {
            return converter(value);
        }

        if (type.GetTypeInfo().IsEnum && (valueType.GetTypeInfo().IsPrimitive || valueType.GetTypeInfo().IsEnum))
        {
            var underlyingType = Enum.GetUnderlyingType(type);

            if (TypeConverters.TryGetValue(underlyingType, out var c))
            {
                return Enum.ToObject(type, c(value));
            }

            return Enum.ToObject(type, Convert.ToUInt64(value));
        }

        return value;
    }

    public static IEnumerable<Type> GetHierarchyGenericArguments(this Type type)
    {
        var genericTypes = type.GetGenericArguments();

        foreach (var genericType in genericTypes)
        {
            yield return genericType;
        }

        var typeInfo = type.GetTypeInfo();
        var baseType = typeInfo.BaseType;

        if (baseType == null)
        {
            yield break;
        }

        var baseGenericArguments = baseType.GetHierarchyGenericArguments();

        foreach (var baseGenericArgument in baseGenericArguments)
        {
            yield return baseGenericArgument;
        }
    }
}
