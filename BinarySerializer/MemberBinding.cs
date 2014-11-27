using System;

namespace BinarySerialization
{
    /// <summary>
    /// Defines a binding to a member.
    /// </summary>
    /// <seealso cref="BinarySerializer"/>
    public class MemberBinding
    {
        /// <summary>
        /// Initializes a new MemberBinding.
        /// </summary>
        internal MemberBinding()
        {
        }

        /// <summary>
        /// Initializes a new MemberBinding with a path, a source binding mode, and an optional converter. 
        /// </summary>
        /// <param name="path">The path to the source member.</param>
        /// <param name="mode">The source mode.</param>
        /// <param name="converterType">An optional converter.</param>
        /// <param name="converterParameter">An optional converter parameter.</param>
        internal MemberBinding(string path, RelativeSourceMode mode, Type converterType = null, object converterParameter = null)
        {
            Path = path;
            Mode = mode;
            ConverterType = converterType;
            ConverterParameter = converterParameter;
        }

        /// <summary>
        /// Initializes a new MemberBinding with a path and an optional converter.
        /// using mode <see cref="RelativeSourceMode.Self"/>.
        /// </summary>
        /// <param name="path">The path to the source member.</param>
        /// <param name="converterType">An optional converter.</param>
        /// <param name="converterParameter">An optional converter parameter.</param>
        internal MemberBinding(string path, Type converterType = null, object converterParameter = null)
            : this(path, RelativeSourceMode.Self, converterType, converterParameter)
        {
        }

        /// <summary>
        /// Gets or sets the path to the binding source member.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the level of ancestor to look for, in <see cref="RelativeSourceMode.FindAncestor"/> mode. 
        /// Use 1 to indicate the one nearest to the binding target element.
        /// </summary>
        public int AncestorLevel { get; set; }

        /// <summary>
        /// Gets or sets the type of ancestor to look for.
        /// </summary>
        public Type AncestorType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RelativeSourceMode"/> value that describes the location of the
        /// binding source member relative to the position of the binding target.
        /// </summary>
        public RelativeSourceMode Mode { get; set; }

        /// <summary>
        /// An optional converter to be used converting from the source value to the target binding.
        /// </summary>
        public Type ConverterType { get; set; }

        /// <summary>
        /// An optional converter parameter to be passed to the converter.
        /// </summary>
        public object ConverterParameter { get; set; }
    }
}
