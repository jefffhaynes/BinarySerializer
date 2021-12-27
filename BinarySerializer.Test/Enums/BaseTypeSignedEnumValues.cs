namespace BinarySerialization.Test.Enums;

public enum BaseTypeSignedEnumValues : short
{
    PositiveValue = 1,
    NegativeValue = unchecked((short)0xFFFF)
}
