using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BinarySerialization
{
    /// <summary>
    ///     Statically generated accessor delegates for a single serializable member, produced by the
    ///     BinarySerializer.SourceGeneration package.  When present, these delegates replace the
    ///     reflection-based (or dynamically compiled) accessor construction at type graph build time.
    /// </summary>
    public sealed class GeneratedMemberAccessors
    {
        /// <summary>
        ///     Initializes a new instance of the GeneratedMemberAccessors class.
        /// </summary>
        /// <param name="memberName">The case-sensitive member (property or field) name.</param>
        /// <param name="getter">The value getter, or null if the member has no public getter.</param>
        /// <param name="setter">The value setter, or null if the member has no public setter.</param>
        public GeneratedMemberAccessors(string memberName, Func<object, object> getter, Action<object, object> setter)
        {
            MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            Getter = getter;
            Setter = setter;
        }

        /// <summary>
        ///     The member (property or field) name.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        ///     The value getter delegate.
        /// </summary>
        public Func<object, object> Getter { get; }

        /// <summary>
        ///     The value setter delegate.
        /// </summary>
        public Action<object, object> Setter { get; }
    }

    /// <summary>
    ///     Registry of source-generated static serialization metadata.  The optional
    ///     BinarySerializer.SourceGeneration package emits calls into this registry from a module
    ///     initializer, providing statically rooted types, constructor factories and member accessors.
    ///     The serializer prefers registered data over runtime reflection and falls back to the
    ///     reflection discovery path for anything not registered, so behavior is identical either way.
    /// </summary>
    public static class GeneratedTypeRegistry
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Factories =
            new ConcurrentDictionary<Type, Func<object>>();

        private static readonly ConcurrentDictionary<(Type, string), GeneratedMemberAccessors> Accessors =
            new ConcurrentDictionary<(Type, string), GeneratedMemberAccessors>();

        /// <summary>
        ///     Registers source-generated metadata for a type.  Calling this method statically roots
        ///     all members of <paramref name="type" /> for trimming and AOT compilation.
        /// </summary>
        /// <param name="type">The type being registered.</param>
        /// <param name="parameterlessConstructor">
        ///     A factory producing a new instance via the parameterless constructor, or null when the
        ///     type has no public parameterless constructor.
        /// </param>
        /// <param name="members">Accessor delegates for the serializable members declared on the type.</param>
        public static void Register(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
            Func<object> parameterlessConstructor,
            params GeneratedMemberAccessors[] members)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (parameterlessConstructor != null)
            {
                Factories.TryAdd(type, parameterlessConstructor);
            }

            if (members == null)
            {
                return;
            }

            foreach (var member in members)
            {
                if (member?.MemberName != null)
                {
                    Accessors.TryAdd((type, member.MemberName), member);
                }
            }
        }

        /// <summary>
        ///     Roots a type for trimming and AOT compilation without registering member accessors.
        ///     Used for types the serializer reflects over indirectly (closed generic collections,
        ///     subtype targets, converter and factory implementations, context objects, and so on).
        /// </summary>
        /// <param name="type">The type to root.</param>
        public static void Keep([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            /* Static reference only: the annotation on the parameter makes the trimmer or AOT
               compiler preserve all members of the type supplied at the call site. */
        }

        /// <summary>
        ///     Roots an attribute type so that custom attribute instances survive trimming.  Only the
        ///     public parameterless constructor needs rooting; attribute members are called directly
        ///     by the serializer and are kept by normal reachability.
        /// </summary>
        /// <param name="type">The attribute type to root.</param>
        public static void KeepAttribute(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
        {
            /* Static reference only: the annotation on the parameter makes the trimmer or AOT
               compiler preserve the attribute constructor of the type supplied at the call site. */
        }

        /// <summary>
        ///     Attempts to get source-generated accessor delegates for a member.
        /// </summary>
        /// <param name="type">The declaring type of the member.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="getter">The getter delegate, or null if none was registered.</param>
        /// <param name="setter">The setter delegate, or null if none was registered.</param>
        /// <returns>True if accessors were registered for the member.</returns>
        public static bool TryGetAccessors(Type type, string memberName,
            out Func<object, object> getter, out Action<object, object> setter)
        {
            getter = null;
            setter = null;

            if (type == null || memberName == null)
            {
                return false;
            }

            if (!Accessors.TryGetValue((type, memberName), out var accessors))
            {
                return false;
            }

            getter = accessors.Getter;
            setter = accessors.Setter;
            return true;
        }

        /// <summary>
        ///     Attempts to get a source-generated parameterless constructor factory for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="constructor">The factory, or null if none was registered.</param>
        /// <returns>True if a factory was registered for the type.</returns>
        public static bool TryGetParameterlessConstructor(Type type, out Func<object> constructor)
        {
            constructor = null;
            return type != null && Factories.TryGetValue(type, out constructor);
        }
    }
}
