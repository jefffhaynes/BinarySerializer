using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Marks a type as a root for BinarySerializer source generation.  Apply this to serialized
    ///     types that carry no other BinarySerializer attributes so that the
    ///     BinarySerializer.SourceGeneration package includes them in static registration and
    ///     trimming/AOT rooting.  Types that already use BinarySerializer attributes (FieldOrder,
    ///     Subtype, and so on) are discovered automatically and do not need this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class BinarySerializerRootAttribute : Attribute
    {
    }
}
