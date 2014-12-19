using System;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    /// <summary>
    /// Used to represent context for the current serializtion operation, such as the serialization ancestory.
    /// <seealso cref="IBinarySerializable"/>
    /// </summary>
	public class BinarySerializationContext
    {
        private const char PathSeparator = '.';

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
        
        /// <summary>
        /// Get value relative to an object by resolving the binding.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="binding">The target binding.</param>
        /// <returns></returns>
        internal object GetValue(object source, BindingInfo binding)
        {
            MemberInfo memberInfo;
            return GetValue(source, binding, out memberInfo);
        }

        /// <summary>
        /// Get value relative to an object by resolving the binding.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="binding">The target binding.</param>
        /// <param name="memberInfo">The target site.</param>
        /// <returns></returns>
        private object GetValue(object source, BindingInfo binding, out MemberInfo memberInfo)
        {
            object relativeSource = null;

            /* Get source object */
            switch (binding.Mode)
            {
                case RelativeSourceMode.Self:
                    relativeSource = source;
                    break;
                case RelativeSourceMode.PreviousData:
                    relativeSource = PreviousItem;
                    break;
                case RelativeSourceMode.FindAncestor:
                    relativeSource = FindAncestor(binding);
                    break;
            }

            if (relativeSource == null)
            {
                memberInfo = null;
                return null;
            }

            string path = binding.Path;

            var value = GetValue(path, relativeSource, out memberInfo);

            /* Convert, if a converterType was specified */
            if (binding.ConverterType == null)
                return value;

            var converter = Activator.CreateInstance(binding.ConverterType) as IValueConverter;

            if (converter == null)
            {
                var message = string.Format("{0} does not implement IValueConverter.", binding.ConverterType);
                throw new InvalidOperationException(message);
            }

            return converter.Convert(value, binding.ConverterParameter, this);
        }

        /// <summary>
        /// Set value relative to an object by resolving the binding.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="binding">The target binding.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        internal void SetValue(object source, BindingInfo binding, object value)
        {
            object relativeSource = null;

            /* Get source object */
            switch (binding.Mode)
            {
                case RelativeSourceMode.Self:
                    relativeSource = source;
                    break;
                case RelativeSourceMode.PreviousData:
                    relativeSource = PreviousItem;
                    break;
                case RelativeSourceMode.FindAncestor:
                    relativeSource = FindAncestor(binding);
                    break;
            }

            if (relativeSource == null)
                return;

            string path = binding.Path;

            /* Convert, if a converterType was specified */
            if (binding.ConverterType != null)
            {
                var converter = Activator.CreateInstance(binding.ConverterType) as IValueConverter;

                if (converter == null)
                {
                    var message = string.Format("{0} does not implement IValueConverter.", binding.ConverterType);
                    throw new InvalidOperationException(message);
                }

                value = converter.ConvertBack(value, binding.ConverterParameter, this);
            }

            SetValue(path, relativeSource, value);
        }

        private object FindAncestor(BindingInfo binding)
        {
            int level = 1;
            BinarySerializationContext parentCtx = this;
            while (parentCtx != null)
            {
                if (binding.AncestorLevel == level || parentCtx.ParentType == binding.AncestorType)
                {
                    return parentCtx.Parent;
                }

                parentCtx = parentCtx.ParentContext;
                level++;
            }

            return null;
        }

        private static object GetValue(string path, object source, out MemberInfo memberInfo)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (source == null)
                throw new ArgumentNullException("source");

            /* Get various members along path */
            string[] memberNames = path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new ArgumentException("Paths cannot be empty.");

            memberInfo = null;

            /* Walk path */
            object child = source;
            foreach (string memberName in memberNames)
            {
                memberInfo = child.GetType().GetMember(memberName).SingleOrDefault();

                if (memberInfo == null)
                {
                    var message = string.Format("Could not resolve path '{0}'.", path);
                    throw new InvalidOperationException(message);
                }

                child = memberInfo.GetValue(child);
            }

            return child;
        }

        private static void SetValue(string path, object source, object value)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (source == null)
                throw new ArgumentNullException("source");

            /* Get various members along path */
            string[] memberNames = path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new ArgumentException("Paths cannot be empty.");

            MemberInfo memberInfo = null;

            /* Walk path */
            object parent = null;
            object child = source;
            foreach (string memberName in memberNames)
            {
                parent = child;
                memberInfo = parent.GetType().GetMember(memberName).SingleOrDefault();

                if (memberInfo == null)
                {
                    var message = string.Format("Could not resolve path '{0}'.", path);
                    throw new InvalidOperationException(message);
                }

                child = memberInfo.GetValue(parent);
            }

            memberInfo.SetValue(parent, value);
        }
	}
}

