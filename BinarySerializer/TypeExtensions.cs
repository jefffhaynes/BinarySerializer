using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BinarySerialization
{
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


        /// <summary>
        ///     Creates an array of the specified element type and size.  Primitive element types are created with
        ///     statically referenced array code so this works when AOT compiling; other element types fall back to
        ///     <see cref="Array.CreateInstance(Type, int)" /> and require the array type to be statically rooted in
        ///     trimmed or AOT-compiled applications (see README, AOT support).
        /// </summary>
        [UnconditionalSuppressMessage("AOT", "IL3050",
            Justification =
                "Primitive element arrays take the statically linked path.  Array.CreateInstance is only used for other element types, whose array types are preserved when the owning serialized types are rooted (see README, AOT support).")]
        internal static Array CreateArray(Type elementType, int size)
        {
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.Boolean:
                    return new bool[size];
                case TypeCode.Byte:
                    return new byte[size];
                case TypeCode.SByte:
                    return new sbyte[size];
                case TypeCode.Char:
                    return new char[size];
                case TypeCode.Int16:
                    return new short[size];
                case TypeCode.UInt16:
                    return new ushort[size];
                case TypeCode.Int32:
                    return new int[size];
                case TypeCode.UInt32:
                    return new uint[size];
                case TypeCode.Int64:
                    return new long[size];
                case TypeCode.UInt64:
                    return new ulong[size];
                case TypeCode.Single:
                    return new float[size];
                case TypeCode.Double:
                    return new double[size];
                case TypeCode.String:
                    return new string[size];
                case TypeCode.Decimal:
                    return new decimal[size];
                case TypeCode.DateTime:
                    return new DateTime[size];
            }

            if (elementType == typeof(IntPtr))
            {
                return new IntPtr[size];
            }

            if (elementType == typeof(UIntPtr))
            {
                return new UIntPtr[size];
            }

            return Array.CreateInstance(elementType, size);
        }

        /// <summary>
        ///     Gets the public parameterless constructor for a type, or null if none exists.
        /// </summary>
        internal static ConstructorInfo GetParameterlessConstructor(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        /// <summary>
        ///     Creates a constructor delegate for an empty <see cref="List{T}" /> without relying on constructor
        ///     reflection metadata, which is not reliably populated for constructed generic types when AOT
        ///     compiling.  Primitive element types use statically referenced factories; other reference type
        ///     elements use a shared-code generic helper.  Returns null for other collection types so callers
        ///     can fall back to constructor reflection.
        /// </summary>
        internal static Func<object> CreateListConstructor(Type type)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
            {
                return null;
            }

            var elementType = type.GetGenericArguments()[0];

            if (PrimitiveListFactories.TryGetValue(elementType, out var factory))
            {
                return factory;
            }

            if (!elementType.IsValueType)
            {
                /* Reference type element: shared code makes this instantiation available when AOT compiling. */
                return MakeGenericListFactory(elementType);
            }

            return null;
        }

        private static readonly Dictionary<Type, Func<object>> PrimitiveListFactories =
            new Dictionary<Type, Func<object>>
            {
                {typeof(bool), () => new List<bool>()},
                {typeof(byte), () => new List<byte>()},
                {typeof(sbyte), () => new List<sbyte>()},
                {typeof(char), () => new List<char>()},
                {typeof(short), () => new List<short>()},
                {typeof(ushort), () => new List<ushort>()},
                {typeof(int), () => new List<int>()},
                {typeof(uint), () => new List<uint>()},
                {typeof(long), () => new List<long>()},
                {typeof(ulong), () => new List<ulong>()},
                {typeof(float), () => new List<float>()},
                {typeof(double), () => new List<double>()},
                {typeof(string), () => new List<string>()},
                {typeof(decimal), () => new List<decimal>()},
                {typeof(DateTime), () => new List<DateTime>()},
                {typeof(object), () => new List<object>()}
            };

        [UnconditionalSuppressMessage("AOT", "IL3050",
            Justification =
                "MakeGenericMethod is used with reference type arguments, which are supported on Native AOT through shared code.  Failures for uncompiled value type instantiations are caught and fall back to constructor reflection.")]
        private static Func<object> MakeGenericListFactory(Type elementType)
        {
            try
            {
                var helper = typeof(TypeExtensions).GetMethod(nameof(CreateListHelper),
                    BindingFlags.Static | BindingFlags.NonPublic);
                return (Func<object>) helper.MakeGenericMethod(elementType)
                    .Invoke(null, null);
            }
            catch (Exception)
            {
                /* Value type instantiations may not be compiled in; callers fall back to constructor reflection. */
                return null;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static Func<object> CreateListHelper<T>()
        {
            return () => new List<T>();
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
}