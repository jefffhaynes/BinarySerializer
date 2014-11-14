using BinarySerialization;

namespace BinarySerializer.Test.Enums
{
    public enum NamedEnumValues
    {
        [SerializeAsEnum("Alpha")]
        A,
        [SerializeAsEnum("Bravo")]
        B,
        [SerializeAsEnum]
        C
    }
}