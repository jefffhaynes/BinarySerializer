namespace BinarySerialization;

/// <summary>
///     Used in conjunction with one or more ItemSubtype attributes to specify the default type to use during
///     deserialization.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ItemSubtypeDefaultAttribute : SubtypeDefaultBaseAttribute
{
    /// <summary>
    ///     Initializes a new instance of <see cref="SubtypeDefaultAttribute" />.
    /// </summary>
    /// <param name="subtype">The default subtype.</param>
    public ItemSubtypeDefaultAttribute(Type subtype) : base(subtype)
    {
    }
}
