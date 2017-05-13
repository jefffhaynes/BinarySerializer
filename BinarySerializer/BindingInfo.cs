using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Defines a binding to a member.
    /// </summary>
    /// <seealso cref="BinarySerializer" />
    public class BindingInfo
    {
        /// <summary>
        ///     Initializes a new BindingInfo.
        /// </summary>
        internal BindingInfo()
        {
        }

        /// <summary>
        ///     Initializes a new BindingInfo with a path, a source binding RelativeSourceMode, and an optional converter.
        /// </summary>
        /// <param name="path">The path to the source member.</param>
        /// <param name="bindingMode">The binding direction.</param>
        /// <param name="relativeSourceMode">The source RelativeSourceMode.</param>
        /// <param name="converterType">An optional converter.</param>
        /// <param name="converterParameter">An optional converter parameter.</param>
        internal BindingInfo(string path, BindingMode bindingMode, RelativeSourceMode relativeSourceMode,
            Type converterType = null,
            object converterParameter = null)
        {
            Path = path;
            BindingMode = bindingMode;
            RelativeSourceMode = relativeSourceMode;
            ConverterType = converterType;
            ConverterParameter = converterParameter;
        }

        /// <summary>
        ///     Initializes a new BindingInfo with a path and an optional converter.
        ///     using RelativeSourceMode <see cref="BinarySerialization.RelativeSourceMode.Self" />.
        /// </summary>
        /// <param name="path">The path to the source member.</param>
        /// <param name="converterType">An optional converter.</param>
        /// <param name="converterParameter">An optional converter parameter.</param>
        internal BindingInfo(string path, Type converterType = null, object converterParameter = null)
            : this(path, BindingMode.TwoWay, RelativeSourceMode.Self, converterType, converterParameter)
        {
        }

        /// <summary>
        ///     Gets or sets the path to the binding source member.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Direction of the binding.
        /// </summary>
        public BindingMode BindingMode { get; set; }

        /// <summary>
        ///     Gets or sets the level of ancestor to look for, in
        ///     <see cref="BinarySerialization.RelativeSourceMode.FindAncestor" /> RelativeSourceMode.
        ///     Use 1 to indicate the one nearest to the binding target element.
        /// </summary>
        public int AncestorLevel { get; set; }

        /// <summary>
        ///     Gets or sets the type of ancestor to look for.
        /// </summary>
        public Type AncestorType { get; set; }

        /// <summary>
        ///     Gets or sets a <see cref="BinarySerialization.RelativeSourceMode" /> value that describes the location of the
        ///     binding source member relative to the position of the binding target.
        /// </summary>
        public RelativeSourceMode RelativeSourceMode { get; set; }

        /// <summary>
        ///     An optional converter to be used converting from the source value to the target binding.
        /// </summary>
        public Type ConverterType { get; set; }

        /// <summary>
        ///     An optional converter parameter to be passed to the converter.
        /// </summary>
        public object ConverterParameter { get; set; }

        /// <summary>
        ///     Returns true if two bindings are equivalent.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((BindingInfo) obj);
        }

        /// <summary>
        ///     Get the hash value for this binding.
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        ///     Returns true if two bindings are equivalent.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(BindingInfo other)
        {
            return string.Equals(Path, other.Path) && BindingMode == other.BindingMode &&
                   AncestorLevel == other.AncestorLevel &&
                   AncestorType == other.AncestorType && ConverterType == other.ConverterType &&
                   RelativeSourceMode == other.RelativeSourceMode &&
                   Equals(ConverterParameter, other.ConverterParameter);
        }
    }
}