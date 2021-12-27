namespace BinarySerialization;

/// <summary>
///     Used to control conditional serialization of members.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class SerializeWhenAttribute : FieldBindingBaseAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializeWhenAttribute" />.
    /// </summary>
    /// <param name="valuePath">The path to the binding source.</param>
    /// <param name="value">The value to be used in determining if the condition is true.</param>
    /// <param name="operator">The comparison operator used to determine serialization eligibility.</param>
    public SerializeWhenAttribute(string valuePath, object value, ComparisonOperator @operator = ComparisonOperator.Equal) : base(valuePath)
    {
        Value = value;
        Operator = @operator;
    }

    /// <summary>
    ///     The value to be used in determining if the condition is true.
    /// </summary>
    public object Value { get; set; }

    public ComparisonOperator Operator { get; }
}
