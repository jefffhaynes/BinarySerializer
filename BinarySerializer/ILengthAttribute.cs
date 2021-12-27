namespace BinarySerialization;

/// <summary>
///     Implemented by attributes that support field binding and field length binding.
/// </summary>
internal interface ILengthAttribute : IBindableFieldAttribute
{
    /// <summary>
    ///     The length of the member for fixed-length fields.  This value will be used if no binding is specified.
    /// </summary>
    ulong ConstLength { get; }
}
