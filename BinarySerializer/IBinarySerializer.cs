namespace BinarySerialization;

/// <summary>
/// Serializes and deserializes an object, or a graph of connected objects, in binary format.
/// </summary>
public interface IBinarySerializer
{
    /// <summary>
    ///     The default <see cref="Endianness" /> to use during serialization or deserialization.
    ///     This property can be updated during serialization or deserialization operations as needed.
    /// </summary>
    Endianness Endianness { get; set; }

    /// <summary>
    ///     The default Encoding to use during serialization or deserialization.
    ///     This property can be updated during serialization or deserialization operations as needed.
    /// </summary>
    Encoding Encoding { get; set; }

    /// <summary>
    ///     Occurs after a member has been serialized.
    /// </summary>
    event EventHandler<MemberSerializedEventArgs> MemberSerialized;

    /// <summary>
    ///     Occurs after a member has been deserialized.
    /// </summary>
    event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

    /// <summary>
    ///     Occurs before a member has been serialized.
    /// </summary>
    event EventHandler<MemberSerializingEventArgs> MemberSerializing;

    /// <summary>
    ///     Occurs before a member has been deserialized.
    /// </summary>
    event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

    /// <summary>
    ///     Serializes the object, or graph of objects with the specified top (root), to the given stream.
    /// </summary>
    /// <param name="stream">The stream to which the graph is to be serialized.</param>
    /// <param name="value">The object at the root of the graph to serialize.</param>
    /// <param name="context">An optional serialization context.</param>
    void Serialize(Stream stream, object value, object context = null);

    /// <summary>
    ///     Calculates the serialized length of the object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <param name="context">An optional context.</param>
    /// <returns>The length of the specified object graph when serialized.</returns>
    long SizeOf(object value, object context = null);

    /// <summary>
    ///     Deserializes the specified stream into an object graph.
    /// </summary>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="type">The type of the root of the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <returns>The deserialized object graph.</returns>
    object Deserialize(Stream stream, Type type, object context = null);

    /// <summary>
    ///     Deserializes the specified stream into an object graph using the stream async methods.
    /// </summary>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="type">The type of the root of the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The deserialized object graph.</returns>
    Task<object> DeserializeAsync(Stream stream, Type type, object context,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deserializes the specified stream into an object graph using the stream async methods.
    /// </summary>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="type">The type of the root of the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <returns>The deserialized object graph.</returns>
    Task<object> DeserializeAsync(Stream stream, Type type, object context = null);

    /// <summary>
    ///     Deserializes the specified stream into an object graph using the stream async methods.
    /// </summary>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="type">The type of the root of the object graph.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The deserialized object graph.</returns>
    Task<object> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken);

    /// <summary>
    ///     Deserializes the specified stream into an object graph using the stream async methods.
    /// </summary>
    /// <typeparam name="T">The type of the root of the object graph.</typeparam>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <returns>The deserialized object graph.</returns>
    Task<T> DeserializeAsync<T>(Stream stream, object context = null);

    /// <summary>
    ///     Deserializes the specified stream into an object graph using the stream async methods.
    /// </summary>
    /// <typeparam name="T">The type of the root of the object graph.</typeparam>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The deserialized object graph.</returns>
    Task<T> DeserializeAsync<T>(Stream stream, object context, CancellationToken cancellationToken);

    /// <summary>
    ///     Deserializes the specified stream into an object graph using the stream async methods.
    /// </summary>
    /// <typeparam name="T">The type of the root of the object graph.</typeparam>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The deserialized object graph.</returns>
    Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    ///     Deserializes the specified stream into an object graph.
    /// </summary>
    /// <param name="data">The byte array from which to deserialize the object graph.</param>
    /// <param name="type">The type of the root of the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <returns>The deserialized object graph.</returns>
    object Deserialize(byte[] data, Type type, object context = null);

    /// <summary>
    ///     Deserializes the specified stream into an object graph.
    /// </summary>
    /// <typeparam name="T">The type of the root of the object graph.</typeparam>
    /// <param name="stream">The stream from which to deserialize the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <returns>The deserialized object graph.</returns>
    T Deserialize<T>(Stream stream, object context = null);

    /// <summary>
    ///     Deserializes the specified stream into an object graph.
    /// </summary>
    /// <typeparam name="T">The type of the root of the object graph.</typeparam>
    /// <param name="data">The byte array from which to deserialize the object graph.</param>
    /// <param name="context">An optional serialization context.</param>
    /// <returns>The deserialized object graph.</returns>
    T Deserialize<T>(byte[] data, object context = null);
}
