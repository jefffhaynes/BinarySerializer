using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal abstract class ContainerTypeNode : TypeNode
    {
        protected const BindingFlags ConstructorBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        protected ContainerTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
            : base(parent, type)
        {
        }

        protected ContainerTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type subType = null)
            : base(parent, parentType, memberInfo, subType)
        {
        }

        protected TypeNode GenerateChild([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            try
            {
                ThrowOnBadType(type);

                return CreateNode(type, type, null);
            }
            catch (Exception exception)
            {
                var message = $"There was an error reflecting type '{type}'";
                throw new InvalidOperationException(message, exception);
            }
        }

        protected TypeNode GenerateChild([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo)
        {
            var memberType = GetMemberType(memberInfo);

            try
            {
                ThrowOnBadType(memberType);

                return CreateNode(memberType, parentType, memberInfo);
            }
            catch (Exception exception)
            {
                var message = $"There was an error reflecting member '{memberInfo.Name}'";
                throw new InvalidOperationException(message, exception);
            }
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        [UnconditionalSuppressMessage("Trimming", "IL2063",
            Justification =
                "Member types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        [UnconditionalSuppressMessage("Trimming", "IL2073",
            Justification =
                "Member types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        [UnconditionalSuppressMessage("Trimming", "IL2075",
            Justification =
                "Member types are discovered via reflection over the user object graph.  Users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        protected static Type GetMemberType(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    return propertyInfo.PropertyType;

                case FieldInfo fieldInfo:
                    return fieldInfo.FieldType;
            }

            throw new NotSupportedException($"{memberInfo.GetType().Name} not supported");
        }

// ReSharper disable UnusedParameter.Local
        private static void ThrowOnBadType(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Cannot serialize objects that implement IDictionary.");
            }
        }
// ReSharper restore UnusedParameter.Local

        /// <summary>
        ///     Creates the type node for <paramref name="memberType" />.  This is a static factory instead of
        ///     <c>Activator.CreateInstance</c> so that all node constructors are statically referenced and
        ///     preserved when trimming or AOT compiling.
        /// </summary>
        private TypeNode CreateNode(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type memberType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo)
        {
            var nullableType = Nullable.GetUnderlyingType(memberType);

            var effectiveType = nullableType ?? memberType;

            if (effectiveType.GetTypeInfo().IsEnum)
            {
                return memberInfo == null
                    ? new EnumTypeNode(this, parentType)
                    : new EnumTypeNode(this, parentType, memberInfo);
            }

            if (IsValueType(effectiveType))
            {
                return memberInfo == null
                    ? new ValueTypeNode(this, parentType)
                    : new ValueTypeNode(this, parentType, memberInfo);
            }

            if (memberType.IsArray)
            {
                return memberInfo == null
                    ? new ArrayTypeNode(this, parentType)
                    : new ArrayTypeNode(this, parentType, memberInfo);
            }

            if (typeof(IList).IsAssignableFrom(memberType))
            {
                return memberInfo == null
                    ? new ListTypeNode(this, parentType)
                    : new ListTypeNode(this, parentType, memberInfo);
            }

            if (typeof(Stream).IsAssignableFrom(memberType))
            {
                return memberInfo == null
                    ? new StreamTypeNode(this, parentType)
                    : new StreamTypeNode(this, parentType, memberInfo);
            }

            if (typeof(IBinarySerializable).IsAssignableFrom(memberType))
            {
                return memberInfo == null
                    ? new CustomTypeNode(this, parentType)
                    : new CustomTypeNode(this, parentType, memberInfo);
            }

            if (memberType == typeof(object))
            {
                return memberInfo == null
                    ? new UnknownTypeNode(this, parentType)
                    : new UnknownTypeNode(this, parentType, memberInfo);
            }

            return memberInfo == null
                ? new ObjectTypeNode(this, parentType)
                : new ObjectTypeNode(this, parentType, memberInfo);
        }
    }
}
