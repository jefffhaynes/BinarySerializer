using System;
using System.Reflection;

#pragma warning disable 618

namespace BinarySerialization
{
    /// <summary>
    ///     Used to represent context for the current serialization operation, such as the serialization ancestry.
    ///     <seealso cref="IBinarySerializable" />
    /// </summary>
    public class BinarySerializationContext
    {
        internal BinarySerializationContext()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the BinarySerializationContext class with a parent, parentType, and parentContext.
        /// </summary>
        /// <param name="value">The value of the object being serialized.</param>
        /// <param name="parentValue">The parent of this object in the object graph.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="parentContext">The parent object serialization context.</param>
        /// <param name="memberInfo">Member info for this field.</param>
        internal BinarySerializationContext(object value, object parentValue, Type parentType,
            BinarySerializationContext parentContext, MemberInfo memberInfo)
        {
            Value = value;
            ParentValue = parentValue;
            ParentType = parentType;
            ParentContext = parentContext;
            MemberInfo = memberInfo;

            // Deprecated
            Parent = parentValue;
        }

        /// <summary>
        ///     The value of the object being serialized.
        /// </summary>
        public virtual object Value { get; }

        /// <summary>
        ///     The parent value in the object graph of the object being serialized.
        /// </summary>
        public virtual object ParentValue { get; }

        /// <summary>
        ///     The type of the parent.
        /// </summary>
        public virtual Type ParentType { get; }

        /// <summary>
        ///     The parent object serialization context.
        /// </summary>
        public virtual BinarySerializationContext ParentContext { get; }

        /// <summary>
        ///     The member info for the object being serialized.
        /// </summary>
        public virtual MemberInfo MemberInfo { get; }

        /// <summary>
        ///     Depth of the object graph at this context.
        /// </summary>
        public int Depth
        {
            get
            {
                if (ParentContext == null)
                {
                    return 0;
                }

                return ParentContext.Depth + 1;
            }
        }


        /// <summary>
        ///     The parent value in the object graph of the object being serialized.
        /// </summary>
        [Obsolete("Use ParentValue")]
        public virtual object Parent { get; }

        /// <summary>
        ///     Find the first ancestor with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor.</typeparam>
        /// <returns></returns>
        public T FindAncestor<T>() where T : class
        {
            var parentValue = ParentValue;
            var parentType = ParentType;
            var parentContext = ParentContext;

            while (parentType != typeof(T))
            {
                if (parentContext == null)
                {
                    return null;
                }

                parentValue = parentContext.ParentValue;
                parentType = parentContext.ParentType;
                parentContext = parentContext.ParentContext;
            }

            return (T) parentValue;
        }
    }
}