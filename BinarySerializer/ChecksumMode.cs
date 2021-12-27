namespace BinarySerialization;

/// <summary>
///     Used with the FieldChecksum attribute to define the checksum operation.
/// </summary>
public enum ChecksumMode
{
    /// <summary>
    ///     The 2's complement of the sum of all bytes.
    /// </summary>
    TwosComplement,

    /// <summary>
    ///     The sum of all bytes modulo 256.
    /// </summary>
    Modulo256,

    /// <summary>
    ///     The exclusive-or of all bytes.
    /// </summary>
    Xor
}
