using System;
using System.Diagnostics.CodeAnalysis;

namespace BinarySerialization
{
    /// <summary>
    ///     Used in conjunction with one or more Subtype attributes to specify the default type to use during deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SubtypeDefaultAttribute : SubtypeDefaultBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="SubtypeDefaultAttribute" />.
        /// </summary>
        /// <param name="subtype">The default subtype.</param>
        public SubtypeDefaultAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type subtype) : base(subtype)
        {
        }
    }
}