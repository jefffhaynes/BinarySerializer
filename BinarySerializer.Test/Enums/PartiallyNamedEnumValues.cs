using BinarySerialization;

namespace BinarySerializer.Test.Enums
{
    public enum PartiallyNamedEnumValues
    {
        [SerializeAsEnum]
        A,
        [SerializeAsEnum]
        B,
        C
    }
}