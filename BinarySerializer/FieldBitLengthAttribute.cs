﻿namespace BinarySerialization;

/// <summary>
///     Specifies the length of a member or object sub-graph.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public sealed class FieldBitLengthAttribute : FieldBindingBaseAttribute, ILengthAttribute, IConstAttribute
{
    /// <summary>
    ///     Initializes a new instance of the FieldLength class with a constant length.
    /// </summary>
    /// <param name="length">The fixed-size length of the decorated member.</param>
    public FieldBitLengthAttribute(ulong length)
    {
        ConstLength = length;
    }

    /// <summary>
    ///     Initializes a new instance of the FieldBitLength class with a path pointing to a binding source member.
    ///     <param name="path">A path to the source member.</param>
    /// </summary>
    public FieldBitLengthAttribute(string path) : base(path)
    {
    }

    /// <summary>
    ///     Get constant value or null if not constant.
    /// </summary>
    public object GetConstValue()
    {
        return ConstLength;
    }

    /// <summary>
    ///     The number of items in the decorated member for fixed-sized members or object sub-graphs.
    /// </summary>
    public ulong ConstLength { get; }
}
