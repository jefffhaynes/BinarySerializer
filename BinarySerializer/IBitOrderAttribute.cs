namespace BinarySerialization
{
    /// <summary>
    ///     Implemented by attributes that support field binding and field bit order binding.
    /// </summary>
    internal interface IBitOrderAttribute : IBindableFieldAttribute
    {
        /// <summary>
        ///     The order in which to allocate bits to bit field.  This value will be used if no binding is specified.
        /// </summary>
        BitOrder BitOrder { get; }
    }
}
