using System;

namespace BinarySerialization
{
    /// <summary>
    /// Implemented by attributes that support field binding.
    /// </summary>
	public interface IBindableFieldAttribute
    {
        /// <summary>
        /// The field binding.
        /// </summary>
        MemberBinding Binding { get; set; }

        /// <summary>
        /// Gets or sets the path to the binding source member.
        /// </summary>
		string Path { get; set; }

        /// <summary>
        /// Gets or sets the level of ancestor to look for, in <see cref="RelativeSourceMode.FindAncestor"/> mode. 
        /// Use 1 to indicate the one nearest to the binding target element.
        /// </summary>
		int AncestorLevel { get; set; }

        /// <summary>
        /// Gets or sets the type of ancestor to look for.
        /// </summary>
        Type AncestorType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RelativeSourceMode"/> value that describes the location of the
        /// binding source member relative to the position of the binding target.
        /// </summary>
        RelativeSourceMode Mode { get; set; }

        /// <summary>
        /// The optional IValueConverter to use when converting from the binding source.
        /// </summary>
        Type ConverterType { get; set; }
	}
}

