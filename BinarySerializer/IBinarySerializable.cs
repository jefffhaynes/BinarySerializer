namespace BinarySerialization;

/// <summary>
///     This interface can be implemented to enable custom serialization and deserialization when using
///     the <see cref="BinarySerializer" /> when the various serializer attributes are insufficient.
///     <seealso cref="FieldCountAttribute" />
///     <seealso cref="FieldLengthAttribute" />
///     <seealso cref="FieldOffsetAttribute" />
///     <seealso cref="SerializeWhenAttribute" />
/// </summary>
public interface IBinarySerializable
{
    /// <summary>
    ///     This method will be called by the <see cref="BinarySerializer" /> during serialization of the implementor class.
    /// </summary>
    /// <param name="stream">The target stream.</param>
    /// <param name="endianness">The inherited endianness from the parent object.</param>
    /// <param name="serializationContext">The current serialization context.</param>
    void Serialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext);

    /// <summary>
    ///     This method will be called by the <see cref="BinarySerializer" /> during deserialization of the implementor class.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="endianness">The inherited endianness from the parent object.</param>
    /// <param name="serializationContext">The current serialization context.</param>
    void Deserialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext);
}
