using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to represent context for the current serializtion operation, such as the serialization ancestory.
    /// <seealso cref="IBinarySerializable"/>
    /// </summary>
	public class BinarySerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the BinarySerializationContext class with a parent, parentType, and parentContext.
        /// </summary>
        /// <param name="parent">The parent of this object in the object graph.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="parentContext">The parent object serialization context.</param>
        internal BinarySerializationContext(object parent, Type parentType, BinarySerializationContext parentContext)
        {
            Parent = parent;
            ParentType = parentType;
			ParentContext = parentContext;
		}

        /// <summary>
        /// Initializes a new instance of the BinarySerializationContext class with a parent and parentContext.  The parent
        /// type will be inferred from the parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parentContext"></param>
        /// <exception cref="NullReferenceException">Thrown if the parent is null.</exception>
        internal BinarySerializationContext(object parent, BinarySerializationContext parentContext)
            : this(parent, parent.GetType(), parentContext)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the BinarySerializationContext class with a parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <exception cref="NullReferenceException">Thrown if the parent is null.</exception>
        public BinarySerializationContext(object parent)
            : this(parent, parent.GetType(), null)
        {
        }

        /// <summary>
        /// The parent in the object graph of the object being serialized.
        /// </summary>
		public object Parent { get; set; }

        /// <summary>
        /// The type of the parent.
        /// </summary>
		public Type ParentType { get; set; }

        /// <summary>
        /// The parent object serialization context.
        /// </summary>
		public BinarySerializationContext ParentContext { get; set; }
		
        /// <summary>
        /// The previous item if the object being serialized is part of a collection or array.
        /// </summary>
		public object PreviousItem { get; set; }

        /// <summary>
        /// Depth of the object graph at this context.
        /// </summary>
        public int Depth
        {
            get
            {
                if (ParentContext == null)
                    return 0;

                return ParentContext.Depth + 1;
            }
        }

        /// <summary>
        /// Find the first ancestor with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor.</typeparam>
        /// <returns></returns>
        public T FindAncestor<T>() where T : class
        {
            var parent = Parent;
            var parentType = ParentType;
            var parentContext = ParentContext;

            while (parentType != typeof (T))
            {
                if (parentContext == null)
                    return null;

                parent = parentContext.Parent;
                parentType = parentContext.ParentType;
                parentContext = parentContext.ParentContext;
            }

            return (T)parent;
        }
	}
}

