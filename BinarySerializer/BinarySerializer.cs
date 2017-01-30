using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BinarySerialization.Graph.TypeGraph;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization
{
    /// <summary>
    ///     Declaratively serializes and deserializes an object, or a graph of connected objects, in binary format.
    ///     <seealso cref="IgnoreAttribute" />
    ///     <seealso cref="FieldOrderAttribute" />
    ///     <seealso cref="SerializeAsEnumAttribute" />
    ///     <seealso cref="FieldOffsetAttribute" />
    ///     <seealso cref="FieldLengthAttribute" />
    ///     <seealso cref="FieldCountAttribute" />
    ///     <seealso cref="SubtypeAttribute" />
    ///     <seealso cref="SerializeWhenAttribute" />
    ///     <seealso cref="SerializeUntilAttribute" />
    ///     <seealso cref="ItemLengthAttribute" />
    ///     <seealso cref="ItemSerializeUntilAttribute" />
    ///     <seealso cref="IBinarySerializable" />
    /// </summary>
    public class BinarySerializer
    {
        private const Endianness DefaultEndianness = Endianness.Little;
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private static readonly ConcurrentDictionary<Type, RootTypeNode> GraphCache = new ConcurrentDictionary<Type, RootTypeNode>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BinarySerializer()
        {
            Endianness = DefaultEndianness;
            Encoding = DefaultEncoding;
        }

        /// <summary>
        /// The default <see cref="Endianness" /> to use during serialization or deserialization.
        /// This property can be updated during serialization or deserialization operations as needed.
        /// </summary>
        public Endianness Endianness { get; set; }

        /// <summary>
        /// The default Encoding to use during serialization or deserialization.
        /// This property can be updated during serialization or deserialization operations as needed.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized
        {
            add { _eventShuttle.MemberSerialized += value; }
            remove { _eventShuttle.MemberSerialized -= value; }
        }

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized
        {
            add { _eventShuttle.MemberDeserialized += value; }
            remove { _eventShuttle.MemberDeserialized -= value; }
        }

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing
        {
            add { _eventShuttle.MemberSerializing += value; }
            remove { _eventShuttle.MemberSerializing -= value; }
        }

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing
        {
            add { _eventShuttle.MemberDeserializing += value; }
            remove { _eventShuttle.MemberDeserializing -= value; }
        }
        
        private readonly EventShuttle _eventShuttle = new EventShuttle();

        private RootTypeNode GetGraph(Type valueType)
        {
            return GraphCache.GetOrAdd(valueType, type => new RootTypeNode(type));
        }

        /// <summary>
        ///     Serializes the object, or graph of objects with the specified top (root), to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the graph is to be serialized.</param>
        /// <param name="value">The object at the root of the graph to serialize.</param>
        /// <param name="context">An optional serialization context.</param>
        public void Serialize(Stream stream, object value, object context = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (value == null)
                return;

            RootTypeNode graph = GetGraph(value.GetType());

            var serializer = (RootValueNode)graph.CreateSerializer(null);
            serializer.EndiannessCallback = () => Endianness;
            serializer.EncodingCallback = () => Encoding;
            serializer.Value = value;
            serializer.Context = context;
            serializer.Bind();

            serializer.Serialize(new BoundedStream(stream), _eventShuttle);
        }

        /// <summary>
        ///     Calculates the serialized length of the object.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <param name="context">An optional context.</param>
        /// <returns>The length of the specified object graph when serialized.</returns>
        public long SizeOf(object value, object context = null)
        {
            var nullStream = new NullStream();
            Serialize(nullStream, value, context);
            return nullStream.Length;
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public object Deserialize(Stream stream, Type type, object context = null)
        {
            RootTypeNode graph = GetGraph(type);

            var serializer = (RootValueNode)graph.CreateSerializer(null);
            serializer.EndiannessCallback = () => Endianness;
            serializer.EncodingCallback = () => Encoding;
            serializer.Context = context;
            serializer.Deserialize(new BoundedStream(stream), _eventShuttle);

            return serializer.Value;
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <param name="data">The byte array from which to deserialize the object graph.</param>
        /// <param name="type">The type of the root of the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public object Deserialize(byte[] data, Type type, object context = null)
        {
            return Deserialize(new MemoryStream(data), type, context);
        }

        /// <summary>
        ///     Deserializes the specified stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
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
        public T Deserialize<T>(byte[] data, object context = null)
        {
            return Deserialize<T>(new MemoryStream(data), context);
        }
    }
}