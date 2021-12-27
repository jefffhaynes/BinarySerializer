namespace BinarySerialization;

/// <summary>
///     Used to specify multiple possible derived types.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public sealed class SubtypeAttribute : SubtypeBaseAttribute
{
    /// <summary>
    ///     Initializes a new instance of <see cref="SubtypeAttribute" />.
    /// </summary>
    /// <param name="valuePath">The path to the binding source.</param>
    /// <param name="value">The value to be used in determining if the subtype should be used.</param>
    /// <param name="subtype">The subtype to be used.</param>
    public SubtypeAttribute(string valuePath, object value, Type subtype)
        : base(valuePath, value, subtype)
    {
    }
}
