using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used in conjunction with one or more Subtype attributes to specify the default type to use during deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SubtypeDefaultAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SubtypeDefaultAttribute"/>.
        /// </summary>
        /// <param name="subtype">The default subtype.</param>
        public SubtypeDefaultAttribute(Type subtype)
        {
            Subtype = subtype;
        }

        /// <summary>
        /// The default subtype.  This type must be assignable to the field type.
        /// </summary>
        public Type Subtype { get; }
    }
}
