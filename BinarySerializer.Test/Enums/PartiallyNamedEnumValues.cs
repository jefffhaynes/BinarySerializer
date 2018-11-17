namespace BinarySerialization.Test.Enums
{
    public enum PartiallyNamedEnumValues
    {
        [SerializeAsEnum] A,
        [SerializeAsEnum] B,
        C
    }
}