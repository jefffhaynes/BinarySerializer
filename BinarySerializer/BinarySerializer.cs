using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization
{
    /// <summary>
    ///     Declaratively serializes and deserializes an object, or a graph of connected objects, in binary format.
    ///     <seealso cref="IgnoreAttribute" />
    ///     <seealso cref="FieldOrderAttribute" />
    ///     <seealso cref="FieldLengthAttribute" />
    ///     <seealso cref="FieldCountAttribute" />
    ///     <seealso cref="FieldAlignmentAttribute" />
    ///     <seealso cref="FieldScaleAttribute" />
    ///     <seealso cref="FieldEndiannessAttribute" />
    ///     <seealso cref="FieldEncodingAttribute" />
    ///     <seealso cref="FieldValueAttribute" />
    ///     <seealso cref="FieldChecksumAttribute" />
    ///     <seealso cref="FieldCrc16Attribute" />
    ///     <seealso cref="FieldCrc32Attribute" />
    ///     <seealso cref="FieldOffsetAttribute" />
    ///     <seealso cref="SubtypeAttribute" />
    ///     <seealso cref="SubtypeFactoryAttribute" />
    ///     <seealso cref="SubtypeDefaultAttribute" />
    ///     <seealso cref="SerializeAsAttribute" />
    ///     <seealso cref="SerializeAsEnumAttribute" />
    ///     <seealso cref="SerializeWhenAttribute" />
    ///     <seealso cref="SerializeWhenNotAttribute" />
    ///     <seealso cref="SerializeUntilAttribute" />
    ///     <seealso cref="ItemLengthAttribute" />
    ///     <seealso cref="ItemSubtypeAttribute" />
    ///     <seealso cref="ItemSubtypeFactoryAttribute" />
    ///     <seealso cref="ItemSubtypeDefaultAttribute" />
    ///     <seealso cref="ItemSerializeUntilAttribute" />
    ///     <seealso cref="IBinarySerializable" />
    /// </summary>
    public class BinarySerializer : IBinarySerializer
    {
        private const Endianness DefaultEndianness = Endianness.Little;
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private static readonly GraphGenerator GraphGenerator = new GraphGenerator();

        private readonly EventShuttle _eventShuttle = new EventShuttle();

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public BinarySerializer()
        {
            Endianness = DefaultEndianness;
            Encoding = DefaultEncoding;
        }

        /// <summary>
        ///     The default <see cref="Endianness" /> to use during serialization or deserialization.
        ///     This property can be updated during serialization or deserialization operations as needed.
        /// </summary>
        public Endianness Endianness { get; set; }

        /// <summary>
        ///     The default Encoding to use during serialization or deserialization.
        ///     This property can be updated during serialization or deserialization operations as needed.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized
        {
            add => _eventShuttle.MemberSerialized += value;
            remove => _eventShuttle.MemberSerialized -= value;
        }

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized
        {
            add => _eventShuttle.MemberDeserialized += value;
            remove => _eventShuttle.MemberDeserialized -= value;
        }

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing
        {
            add => _eventShuttle.MemberSerializing += value;
            remove => _eventShuttle.MemberSerializing -= value;
        }

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing
        {
            add => _eventShuttle.MemberDeserializing += value;
            remove => _eventShuttle.MemberDeserializing -= value;
        }

        /// <summary>
        ///     Serializes the object, or graph of objects with the specified top (root), to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the graph is to be serialized.</param>
        /// <param name="value">The object at the root of the graph to serialize.</param>
        /// <param name="context">An optional serialization context.</param>
        [Obsolete("Use SerializeAsync")]
        public void Serialize(Stream stream, object value, object context = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value == null)
            {
                return;
            }

            var serializer = CreateSerializer(value.GetType(), context);
            serializer.Value = value;
            serializer.Bind();

            serializer.Serialize(new BoundedStream(stream, "root"), _eventShuttle);
        }

        /// <summary>
        ///     Serializes the object, or graph of objects with the specified top (root), to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the graph is to be serialized.</param>
        /// <param name="value">The object at the root of the graph to serialize.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <param name="cancellationToken"></param>
        public async Task SerializeAsync(Stream stream, object value, object context = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value == null)
            {
                return;
            }

            var serializer = CreateSerializer(value.GetType(), context);
            serializer.Value = value;
            serializer.Bind();

            await serializer.SerializeAsync(new BoundedStream(stream, "root"), _eventShuttle, true, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Calculates the serialized length of the object.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <param name="context">An optional context.</param>
        /// <returns>The length of the specified object graph when serialized.</returns>
        [Obsolete("Use SizeOfAsync")]
        public long SizeOf(object value, object context = null)
        {
            var nullStream = new NullStream();
            Serialize(nullStream, value, context);
            return nullStream.Length;
        }

        /// <summary>
        ///     Calculates the serialized length of the object.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <param name="context">An optional context.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The length of the specified object graph when serialized.</returns>
        public async Task<long> SizeOfAsync(object value, object context = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            var nullStream = new NullStream();
            await SerializeAsync(nullStream, value, context, cancellationToken).ConfigureAwait(false);
            return nullStream.Length;
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        [Obsolete("Use DeserializeAsync")]
        public object Deserialize(Stream stream, Type type, object context = null)
        {
            var serializer = CreateSerializer(type, context);
            serializer.Deserialize(new BoundedStream(stream, "root"), _eventShuttle);

            return serializer.Value;
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph using the stream async methods.
        /// </summary>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The deserialized object graph.</returns>
        public async Task<object> DeserializeAsync(Stream stream, Type type, object context,
            CancellationToken cancellationToken)
        {
            var serializer = CreateSerializer(type, context);
            await serializer.DeserializeAsync(new BoundedStream(stream, "root"), _eventShuttle, cancellationToken)
                .ConfigureAwait(false);

            return serializer.Value;
        }

        private RootValueNode CreateSerializer(Type type, object context)
        {
            var graph = GraphGenerator.GenerateGraph(type);

            var serializer = (RootValueNode) graph.CreateSerializer(null);
            serializer.EndiannessCallback = () => Endianness;
            serializer.EncodingCallback = () => Encoding;
            serializer.Context = context;
            return serializer;
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph using the stream async methods.
        /// </summary>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public Task<object> DeserializeAsync(Stream stream, Type type, object context = null)
        {
            return DeserializeAsync(stream, type, context, CancellationToken.None);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph using the stream async methods.
        /// </summary>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The deserialized object graph.</returns>
        public Task<object> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken)
        {
            return DeserializeAsync(stream, type, null, cancellationToken);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph using the stream async methods.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public async Task<T> DeserializeAsync<T>(Stream stream, object context = null)
        {
            return (T) await DeserializeAsync(stream, typeof(T), context);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph using the stream async methods.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The deserialized object graph.</returns>
        public async Task<T> DeserializeAsync<T>(Stream stream, object context, CancellationToken cancellationToken)
        {
            return (T) await DeserializeAsync(stream, typeof(T), context, cancellationToken);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph using the stream async methods.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The deserialized object graph.</returns>
        public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
        {
            return (T) await DeserializeAsync(stream, typeof(T), cancellationToken);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <param name="data">The byte array from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        [Obsolete("Use DeserializeAsync")]
        public object Deserialize(byte[] data, Type type, object context = null)
        {
            return Deserialize(new MemoryStream(data), type, context);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <param name="data">The byte array from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The deserialized object graph.</returns>
        public Task<object> DeserializeAsync(byte[] data, Type type, object context = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            return DeserializeAsync(new MemoryStream(data), type, context, cancellationToken);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        [Obsolete("Use DeserializeAsync")]
        public T Deserialize<T>(Stream stream, object context = null)
        {
            return (T) Deserialize(stream, typeof(T), context);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="data">The byte array from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        [Obsolete("Use DeserializeAsync")]
        public T Deserialize<T>(byte[] data, object context = null)
        {
            return Deserialize<T>(new MemoryStream(data), context);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="data">The byte array from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public Task<T> DeserializeAsync<T>(byte[] data, object context = null)
        {
            return DeserializeAsync<T>(new MemoryStream(data), context);
        }
    }
}