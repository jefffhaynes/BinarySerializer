using System;

namespace BinarySerialization
{
    /// <summary>
    /// Implemented by attributes that support field binding.
    /// </summary>
	internal interface IBindableFieldAttribute
    {
        /// <summary>
        /// Gets or sets the path to the binding source member.
        /// </summary>
		string Path { get; set; }

        /// <summary>
        /// Gets or sets the level of ancestor to look for, in <see cref="BinarySerialization.RelativeSourceMode.FindAncestor"/> mode. 
        /// Use 1 to indicate the one nearest to the binding target element.
        /// </summary>
		int AncestorLevel { get; set; }

        /// <summary>
        /// Gets or sets the type of ancestor to look for.
        /// </summary>
        Type AncestorType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="BinarySerialization.RelativeSourceMode"/> value that describes the location of the
        /// binding source member relative to the position of the binding target.
        /// </summary>
        RelativeSourceMode RelativeSourceMode { get; set; }

        /// <summary>
        /// Sets a mode defining the directionality of the binding.  Two way by default.
        /// </summary>
        BindingMode BindingMode { get; set; }

        /// <summary>
        /// The optional IValueConverter to use when converting from the binding source.
        /// </summary>
        Type ConverterType { get; set; }

        /// <summary>
        /// An optional parameter to be passed to the converter.
        /// </summary>
        object ConverterParameter { get; set; }
	}
}

