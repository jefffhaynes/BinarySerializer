using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Provides the <see cref="BinarySerializer" /> with information used to serialize the decorated member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldOrderAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldOrderAttribute" /> class.
        /// </summary>
        public FieldOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        ///     The order to be observed when serializing or deserializing
        ///     this member compared to other members in the parent object.
        /// </summary>
        public int Order { get; set; }
    }
}