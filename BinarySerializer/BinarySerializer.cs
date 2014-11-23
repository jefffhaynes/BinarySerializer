using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerialization
{
    /// <summary>
    /// Declaratively serializes and deserializes an object, or a graph of connected objects, in binary format.
    /// <seealso cref="IgnoreAttribute"/>
    /// <seealso cref="SerializeAsAttribute"/>
    /// <seealso cref="SerializeAsEnumAttribute"/>
    /// <seealso cref="FieldOffsetAttribute"/>
    /// <seealso cref="FieldLengthAttribute"/>
    /// <seealso cref="FieldCountAttribute"/>
    /// <seealso cref="SubtypeAttribute"/>
    /// <seealso cref="SerializeWhenAttribute"/>
    /// <seealso cref="SerializeUntilAttribute"/>
    /// <seealso cref="ItemLengthAttribute"/>
    /// <seealso cref="ItemSerializeUntilAttribute"/>
    /// <seealso cref="IBinarySerializable"/>
    /// </summary>
    public class BinarySerializer
    {
        private const long InvalidFieldLength = -1;
        private const long InvalidFieldOffset = -1;
        private static readonly Encoding DefaultEncoding = new UTF8Encoding();

        private static readonly Dictionary<Type, SerializedType> DefaultSerializedTypes =
            new Dictionary<Type, SerializedType>
                {
                    {typeof (bool), SerializedType.Int1},
                    {typeof (sbyte), SerializedType.Int1},
                    {typeof (byte), SerializedType.UInt1},
                    {typeof (char), SerializedType.UInt2},
                    {typeof (short), SerializedType.Int2},
                    {typeof (ushort), SerializedType.UInt2},
                    {typeof (int), SerializedType.Int4},
                    {typeof (uint), SerializedType.UInt4},
                    {typeof (long), SerializedType.Int8},
                    {typeof (ulong), SerializedType.UInt8},
                    {typeof (float), SerializedType.Float4},
                    {typeof (double), SerializedType.Float8},
                    {typeof (string), SerializedType.NullTerminatedString},
                    {typeof (byte[]), SerializedType.ByteArray}
                };

        private bool _firstPass;

        /// <summary>
        /// The default <see cref="Endianness"/> to use during serialization or deserialization.
        /// This property can be updated dynamically during serialization or deserialization.
        /// </summary>
        public Endianness Endianness { get; set; }

        /// <summary>
        /// Occurrs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        /// Occurrs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        /// Occurrs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        /// Occurrs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;
 
        /// <summary>
        /// Serializes the object, or graph of objects with the specified top (root), to the given stream.
        /// </summary>
        /// <param name="stream">The stream to which the graph is to be serialized.</param>
        /// <param name="graph">The object at the root of the graph to serialize.</param>
        /// <param name="context">An optional serialization context.</param>
        public void Serialize(Stream stream, object graph, BinarySerializationContext context = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (graph == null)
                throw new ArgumentNullException("graph", "Object graph cannot be null.");

            var graphType = graph.GetType();

            if (graphType.IsPrimitive)
                WritePrimitive(new EndianAwareBinaryWriter(stream), graph, DefaultSerializedTypes[graphType],
                    InvalidFieldLength, DefaultEncoding);
            else if (graphType.IsList())
                throw new NotSupportedException("Collections cannot be directly serialized (they should be contained).");
            else
            {
                _firstPass = true;

                /* First pass to update source bindings */
                WriteMembers(new NullStream(), graph, graphType, context);
                _firstPass = false;

                /* Ok, serialize. */
                WriteMembers(stream, graph, graphType, context);
            }
        }

        private void WriteMembers(Stream stream, object o, Type objectType, BinarySerializationContext ctx)
        {
            if (o == null)
            {
                var message = string.Format("Null values cannot be represented.  Type {0} cannot be null",
                                            objectType);
                throw new ArgumentNullException("o", message);
            }

            if (typeof (IBinarySerializable).IsAssignableFrom(objectType))
            {
                ((IBinarySerializable) o).Serialize(stream, Endianness, ctx);
            }
            else
            {
                var serializationContext = new BinarySerializationContext(o, objectType, ctx);
                IEnumerable<MemberSerializationInfo> membersSerializationInfo = objectType.GetMembersSerializationInfo();

                foreach (MemberSerializationInfo memberSerializationInfo in membersSerializationInfo)
                {
#if !DEBUG
                    try
#endif
                    {
                        WriteMember(stream, memberSerializationInfo, o, serializationContext);
                    }
#if !DEBUG
                    catch (IOException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Error serializing {0}.  See inner exception for detail.", 
                            memberSerializationInfo.Member.Name);
                        throw new InvalidOperationException(message, e);
                    }
#endif
                }
            }
        }

        private void WriteMember(Stream stream, MemberSerializationInfo memberSerializationInfo, object member,
                                 BinarySerializationContext ctx)
        {
            if (memberSerializationInfo == null)
                throw new ArgumentNullException("memberSerializationInfo");

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (member is IDictionary)
                throw new InvalidOperationException("Cannot serialize objects that implement IDictionary.");

            if (memberSerializationInfo.IgnoreAttribute != null)
                return;

            if (member == null && !memberSerializationInfo.IsLastMember)
            {
                var message = string.Format("'{0}' cannot be null.  Only the final object in a class can be null.",
                                            memberSerializationInfo.Member.Name);
                throw new ArgumentNullException("member", message);
            }

            if (!IsSerializable(memberSerializationInfo, member, ctx))
                return;

            OnMemberSerializing(memberSerializationInfo.Member.Name, ctx);

            var originalPosition = InvalidFieldOffset;
            FieldOffsetAttribute fieldOffsetAttribute = memberSerializationInfo.FieldOffsetAttribute;

            if (fieldOffsetAttribute != null)
            {
                if(!stream.CanSeek)
                    throw new InvalidOperationException("FieldOffsetAttribute not supported for non-seekable streams");
                originalPosition = stream.Position;
                stream.Position = GetOffset(fieldOffsetAttribute, member, ctx);
            }

            object value = memberSerializationInfo.Member.GetValue(member);
            WriteValue(stream, memberSerializationInfo, member, ctx, value);

            /* If we got here via a field offset, restore to original position */
            if (fieldOffsetAttribute != null)
                stream.Position = originalPosition;

            OnMemberSerialized(memberSerializationInfo.Member.Name, value, ctx);
        }

        private void WriteValue(Stream stream, MemberSerializationInfo memberSerializationInfo, object o,
                                BinarySerializationContext ctx, object value)
        {
            if (value == null)
            {
                if (memberSerializationInfo.IsLastMember)
                    return;

                var message = string.Format("'{0}' cannot be null.  Only the final object in a class can be null.",
                                            memberSerializationInfo.Member.Name);
                throw new ArgumentNullException("value", message);
            }

            var valueType = value.GetType();

            /* Look for subtype, if specified */
            var subtypeAttributes = memberSerializationInfo.SubtypeAttributes;
            if (_firstPass && subtypeAttributes.Any())
                SetSubtype(subtypeAttributes, valueType, o, ctx);

            /* We use a stream wrapper that tracks relative position to avoid non-seekable stream issues */
            var streamKeeper = new StreamKeeper(stream);

            var serializedType = SerializedType.Default;
            Encoding encoding = DefaultEncoding;
            Endianness endianness = Endianness;

            SerializeAsAttribute serializeAsAttribute = memberSerializationInfo.SerializeAsAttribute;

            if (serializeAsAttribute != null)
            {
                serializedType = serializeAsAttribute.SerializedType;
                encoding = serializeAsAttribute.Encoding ?? encoding;

                if (serializeAsAttribute.Endianness != Endianness.Inherit)
                    endianness = serializeAsAttribute.Endianness;
            }

            long constLength = InvalidFieldLength;

            FieldLengthAttribute fieldLengthAttribute = memberSerializationInfo.FieldLengthAttribute;

            if (fieldLengthAttribute != null && fieldLengthAttribute.IsConst)
                constLength = (long)fieldLengthAttribute.ConstLength;

            bool isList = ShouldTreatAsList(valueType);

            var position = streamKeeper.RelativePosition;

            if (isList)
            {
                WriteCollection(streamKeeper, memberSerializationInfo, o, ctx, valueType, serializedType, encoding, endianness, constLength);
            }
            else
            {
                WriteValue(streamKeeper, value, valueType, serializedType, encoding, constLength, endianness, ctx);
            }

            /* Back-annotate binding source if necessary (used for first pass) or enforce const */
            if (_firstPass && fieldLengthAttribute != null)
            {
                var length = streamKeeper.RelativePosition - position;

                if (fieldLengthAttribute.IsConst)
                {
                    if (length != constLength)
                    {
                        var message = string.Format(
                                "Byte length value does not match FieldLengthAttribute const value.  Expected {0} but was {1}.",
                                constLength, length);
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    SetValue(fieldLengthAttribute.Binding, o, ctx, length);
                }
            }
        }

        private void WriteCollection(StreamKeeper streamKeeper, MemberSerializationInfo memberSerializationInfo, object o, 
            BinarySerializationContext ctx, Type valueType, SerializedType serializedType, Encoding encoding, Endianness endianness, long constLength)
        {
            Type itemValueType;
            IList list = GetList(memberSerializationInfo, o, valueType, out itemValueType);

            FieldCountAttribute fieldCountAttribute = memberSerializationInfo.FieldCountAttribute;

            if (_firstPass && fieldCountAttribute != null)
            {
                if (fieldCountAttribute.IsConst)
                {
                    if (fieldCountAttribute.ConstCount != list.Count)
                    {
                        var message = string.Format(
                                "List count value does not match FieldCountAttribute const value.  Expected {0} but was {1}.",
                                fieldCountAttribute.ConstCount, list.Count);
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    SetValue(fieldCountAttribute.Binding, o, ctx, list.Count);
                }
            }

            long constItemLength = InvalidFieldLength;

            ItemLengthAttribute itemLengthAttribute = memberSerializationInfo.ItemLengthAttribute;

            if (itemLengthAttribute != null && itemLengthAttribute.IsConst)
                constItemLength = (long) itemLengthAttribute.ConstLength;

            var itemLengths = new List<long>();

            foreach(var item in list)
            {
                var itemPosition = streamKeeper.RelativePosition;
                WriteValue(streamKeeper, item, itemValueType, serializedType, encoding, constItemLength, endianness, ctx);
                var itemLength = streamKeeper.RelativePosition - itemPosition;
                itemLengths.Add(itemLength);
                ctx.PreviousItem = item;
            }

            /* Back-annotate binding source if necessary (used for first pass) */
            if (_firstPass && itemLengthAttribute != null && !itemLengthAttribute.IsConst)
            {
                /* First make sure all items have the same length (or this won't work) */
                var distinctItemLengths = itemLengths.Distinct().ToList();

                if(distinctItemLengths.Count != 1)
                    throw new InvalidOperationException("Can't update ItemLengthAttribute binding source because items have varying lengths.");

                var itemLength = distinctItemLengths[0];

                SetValue(itemLengthAttribute.Binding, o, ctx, itemLength);
            }

            SerializeUntilAttribute serializeUntilAttribute = memberSerializationInfo.SerializeUntilAttribute;

            /* Handle explicit termination case */
            if (serializeUntilAttribute != null)
            {
                var terminationValue = serializeUntilAttribute.Value;
                WriteValue(streamKeeper, terminationValue, terminationValue.GetType(), SerializedType.Default,
                               encoding, constLength, endianness, ctx);
            }

            ctx.PreviousItem = null;
        }

        private void WriteValue(StreamKeeper stream, object value, Type valueType, SerializedType serializedType,
                                Encoding encoding, long constLength, Endianness endianness, BinarySerializationContext ctx)
        {
            var underlyingNullableType = Nullable.GetUnderlyingType(valueType);

            if (valueType.IsPrimitive || valueType.IsEnum || valueType == typeof (string) || valueType == typeof (byte[]) ||
                underlyingNullableType != null)
            {
                if (valueType.IsEnum)
                {
                    SerializedType enumSerializedType;
                    var enumValues = valueType.GetEnumSerializedValues(out enumSerializedType);

                    object enumValue;
                    if (enumValues.TryGetValue((Enum)value, out enumValue))
                        value = enumValue;

                    if (enumSerializedType != SerializedType.Default)
                        serializedType = enumSerializedType;

                    if (serializedType == SerializedType.Default)
                    {
                        Type underlyingType = Enum.GetUnderlyingType(valueType);
                        serializedType = DefaultSerializedTypes[underlyingType];
                    }
                }
                else if (underlyingNullableType != null)
                {
                    serializedType = DefaultSerializedTypes[underlyingNullableType];
                }
                else if (serializedType == SerializedType.Default)
                {
                    serializedType = GetDefaultSerializedType(valueType, constLength);
                }

                var writer = new EndianAwareBinaryWriter(stream, encoding, endianness);

                WritePrimitive(writer, value, serializedType, constLength, encoding);
            }
            else if (typeof(Stream).IsAssignableFrom(valueType))
            {
                var valueStream = (Stream) value;
                var valueStreamlet = constLength == InvalidFieldLength
                                         ? new Streamlet(valueStream)
                                         : new Streamlet(valueStream, valueStream.Position, constLength);
                valueStreamlet.CopyTo(stream);

                if(_firstPass)
                    valueStream.Position = 0;
            }
            else
            {
                WriteMembers(stream, value, valueType, ctx);
            }
        }

        private static void WritePrimitive(EndianAwareBinaryWriter writer, object value, SerializedType serializedType, long length, Encoding encoding)
        {
            switch (serializedType)
            {
                case SerializedType.Int1:
                    writer.Write(Convert.ToSByte(value));
                    break;
                case SerializedType.UInt1:
                    writer.Write(Convert.ToByte(value));
                    break;
                case SerializedType.Int2:
                    writer.Write(Convert.ToInt16(value));
                    break;
                case SerializedType.UInt2:
                    writer.Write(Convert.ToUInt16(value));
                    break;
                case SerializedType.Int4:
                    writer.Write(Convert.ToInt32(value));
                    break;
                case SerializedType.UInt4:
                    writer.Write(Convert.ToUInt32(value));
                    break;
                case SerializedType.Int8:
                    writer.Write(Convert.ToInt64(value));
                    break;
                case SerializedType.UInt8:
                    writer.Write(Convert.ToUInt64(value));
                    break;
                case SerializedType.Float4:
                    writer.Write(Convert.ToSingle(value));
                    break;
                case SerializedType.Float8:
                    writer.Write(Convert.ToDouble(value));
                    break;
                case SerializedType.ByteArray:
                    {
                        var data = (byte[]) value;
                        writer.Write(data, 0, length == InvalidFieldLength ? data.Length : (int) length);
                        break;
                    }
                case SerializedType.NullTerminatedString:
                    {
                        byte[] data = encoding.GetBytes((string) value);
                        writer.Write(data);
                        writer.Write((byte) 0);
                        break;
                    }
                case SerializedType.SizedString:
                    {
                        byte[] data = encoding.GetBytes(value.ToString());

                        if(length != InvalidFieldLength)
                            Array.Resize(ref data, (int) length);

                        writer.Write(data);
                        break;
                    }
                case SerializedType.LengthPrefixedString:
                    writer.Write(Convert.ToString(value));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Calculates the serialized length of the object.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>The length of the specified object graph when serialized.</returns>
        public long SizeOf(object o)
        {
            var nullStream = new NullStream();
            Serialize(nullStream, o);
            return nullStream.Length;
        }
        
        /// <summary>
        /// Deserializes the specified stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="stream">The stream from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public T Deserialize<T>(Stream stream, BinarySerializationContext context = null) where T : new()
        {
            var graphType = typeof (T);

            if (graphType.IsPrimitive)
            {
                var primitive = ReadPrimitive(new EndianAwareBinaryReader(stream), DefaultSerializedTypes[graphType],
                                  DefaultEncoding);
                return (T)Convert.ChangeType(primitive, graphType);
            }

            if (graphType.IsList())
                throw new NotSupportedException(
                    "Collections cannot be directly deserialized (they should be contained).");

            var o = new T();
            ReadMembers(stream, o, graphType, context);
            return o;
        }

        /// <summary>
        /// Deserializes the specified stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type of the root of the object graph.</typeparam>
        /// <param name="data">The byte array from which to deserialize the object graph.</param>
        /// <param name="context">An optional serialization context.</param>
        /// <returns>The deserialized object graph.</returns>
        public T Deserialize<T>(byte[] data, BinarySerializationContext context = null) where T : new()
        {
            return Deserialize<T>(new MemoryStream(data), context);
        }

        private void ReadMembers(Stream stream, object o, Type objectType, BinarySerializationContext ctx)
        {
            if (typeof (IBinarySerializable).IsAssignableFrom(objectType))
            {
                ((IBinarySerializable) o).Deserialize(stream, Endianness, ctx);
            }
            else
            {
                var serializationContext = new BinarySerializationContext(o, objectType, ctx);
                IEnumerable<MemberSerializationInfo> membersSerializationInfo = objectType.GetMembersSerializationInfo();

                foreach (MemberSerializationInfo memberSerializationInfo in membersSerializationInfo)
                {
#if !DEBUG
                    try
#endif
                    {
                        ReadMember(stream, memberSerializationInfo, o, serializationContext);
                    }
#if !DEBUG
                    catch (EndOfStreamException e)
                    {
                        var message = string.Format("Error deserializing {0}.", memberSerializationInfo.Member.Name);
                        throw new InvalidOperationException(message, e);
                    }
                    catch (IOException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        var message = string.Format("Error deserializing {0}.", memberSerializationInfo.Member.Name);
                        throw new InvalidOperationException(message, e);
                    }
#endif
                }
            }
        }

        private void ReadMember(Stream stream, MemberSerializationInfo memberSerializationInfo, object member,
                                BinarySerializationContext ctx)
        {
            if (memberSerializationInfo == null)
                throw new ArgumentNullException("memberSerializationInfo");

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (member == null)
                throw new ArgumentNullException("member");

            if (member is IDictionary)
                throw new InvalidOperationException("Cannot deserialize objects that implement IDictionary.");

            if (memberSerializationInfo.IgnoreAttribute != null)
                return;

            if (!IsSerializable(memberSerializationInfo, member, ctx))
                return;

            OnMemberDeserializing(memberSerializationInfo.Member.Name, ctx);

            long originalPosition = InvalidFieldOffset;
            FieldOffsetAttribute fieldOffsetAttribute = memberSerializationInfo.FieldOffsetAttribute;

            if (fieldOffsetAttribute != null)
            {
                if(!stream.CanSeek)
                    throw new InvalidOperationException("FieldOffsetAttribute not supported for non-seekable streams");
                originalPosition = stream.Position;
                stream.Position = GetOffset(fieldOffsetAttribute, member, ctx);
            }

            Type valueType = memberSerializationInfo.Member.GetMemberType();

            /* Look for subtype, if specified */
            var subtypeAttributes = memberSerializationInfo.SubtypeAttributes;
            if (subtypeAttributes.Any())
                valueType = GetSubtype(subtypeAttributes, valueType, member, ctx);

            var serializedType = SerializedType.Default;
            Encoding encoding = DefaultEncoding;
            Endianness endianness = Endianness;

            SerializeAsAttribute serializeAsAttribute = memberSerializationInfo.SerializeAsAttribute;

            if (serializeAsAttribute != null)
            {
                serializedType = serializeAsAttribute.SerializedType;
                encoding = serializeAsAttribute.Encoding ?? encoding;

                if (serializeAsAttribute.Endianness != Endianness.Inherit)
                    endianness = serializeAsAttribute.Endianness;
            }

            long length = InvalidFieldLength;

            FieldLengthAttribute fieldLengthAttribute = memberSerializationInfo.FieldLengthAttribute;

            if (fieldLengthAttribute != null)
            {
                length = GetLength(fieldLengthAttribute, member, ctx);

                if(length == InvalidFieldLength)
                    throw new InvalidOperationException("Unable to resolve field length.");

                stream = new StreamLimiter(stream, length);
            }

            bool isList = ShouldTreatAsList(valueType);

            if (isList)
            {
                ReadCollection(stream, memberSerializationInfo, member, ctx, valueType, serializedType, encoding, endianness);
            }
            else
            {
                object value = ReadValue(stream, valueType, serializedType, encoding, length, endianness, ctx);

                memberSerializationInfo.Member.SetValue(member, value);
                OnMemberDeserialized(memberSerializationInfo.Member.Name, value, ctx);
            }

            /* If we got here via a field offset, restore to original position */
            if (fieldOffsetAttribute != null)
                stream.Position = originalPosition;
        }
        
        private void ReadCollection(Stream stream, MemberSerializationInfo memberSerializationInfo, object o,
                                    BinarySerializationContext ctx, Type valueType, SerializedType serializedType,
                                    Encoding encoding, Endianness endianness)
        {
            int count;
            Type itemValueType;
            IList list = GetList(memberSerializationInfo, o, ctx, valueType, out itemValueType, out count);

            SerializeUntilAttribute serializeUntilAttribute = memberSerializationInfo.SerializeUntilAttribute;

            long itemLength = InvalidFieldLength;

            ItemLengthAttribute itemLengthAttribute =
                memberSerializationInfo.ItemLengthAttribute;
            if (itemLengthAttribute != null)
            {
                itemLength = GetLength(itemLengthAttribute, o, ctx);

                if (itemLength == InvalidFieldLength)
                    throw new InvalidOperationException("Unable to resolve field length.");
            }

            ItemSerializeUntilAttribute itemSerializeUntilAttribute =
                memberSerializationInfo.ItemSerializeUntilAttribute;

            for (int i = 0; i < count; i++)
            {
                var isEndOfStream = (stream.CanSeek && stream.Position >= stream.Length) ||
                                    (stream is StreamLimiter && (stream as StreamLimiter).Position >= stream.Length);

                /* In the implicit termination case just stop if we hit the end of the stream.  
                     * If the stream isn't seekable, this doesn't apply. */
                if (isEndOfStream)
                    break;

                Stream itemStream = itemLength != InvalidFieldLength ? new StreamLimiter(stream, itemLength) : stream;

                object value = ReadValue(itemStream, itemValueType, serializedType, encoding, itemLength,
                                         endianness, ctx);

                /* Check for item-terminated case */
                if (itemSerializeUntilAttribute != null)
                {
                    object untilValue = new BinarySerializationContext(o, ctx).GetValue(value,
                                                                                        itemSerializeUntilAttribute.Binding);
                    if (untilValue.Equals(itemSerializeUntilAttribute.Value))
                    {
                        if (!itemSerializeUntilAttribute.ExcludeLastItem)
                        {
                            if (valueType.IsArray)
                                list[i] = value;
                            else list.Add(value);
                        }

                        break;
                    }
                }

                if (valueType.IsArray)
                    list[i] = value;
                else list.Add(value);

                /* Check explict termination case */
                if (serializeUntilAttribute != null)
                {
                    if (!stream.CanSeek)
                        throw new InvalidOperationException(
                            "SerializeUntil not supported for non-seekable streams as it requires peek-ahead.");

                    var terminationValue = serializeUntilAttribute.Value;
                    var terminationValueType = terminationValue.GetType();

                    var pos = stream.Position;

                    var nextValue = ReadValue(stream, terminationValueType, SerializedType.Default, encoding, InvalidFieldLength,
                                              endianness, ctx);

                    if (nextValue.Equals(terminationValue))
                        break;

                    stream.Position = pos;
                }

                ctx.PreviousItem = value;
            }

            ctx.PreviousItem = null;
        }

        private object ReadValue(Stream stream, Type valueType, SerializedType serializedType, Encoding encoding,
                                 long constLength, Endianness endianness, BinarySerializationContext ctx)
        {
            object value;

            if (valueType == null)
                return null;

            var underlyingNullableType = Nullable.GetUnderlyingType(valueType);

            if (valueType.IsPrimitive || valueType.IsEnum || valueType == typeof (string) ||
                valueType == typeof (byte[]) || underlyingNullableType != null)
            {
                /* This allows for nullable "optional" members at the end of a length-controlled section */
                var isEndOfStream = (stream.CanSeek && stream.Position >= stream.Length) ||
                                    (stream is StreamLimiter && (stream as StreamLimiter).Position >= stream.Length);

                /* In the implicit termination case just stop if we hit the end of the stream.  
                     * If the stream isn't seekable, this doesn't apply. */
                if (isEndOfStream)
                    return null;

                if (valueType.IsEnum)
                {
                    SerializedType enumSerializedType;
                    valueType.GetEnumSerializedValues(out enumSerializedType);

                    if (enumSerializedType != SerializedType.Default)
                        serializedType = enumSerializedType;

                    if (serializedType == SerializedType.Default)
                    {
                        Type underlyingType = Enum.GetUnderlyingType(valueType);
                        serializedType = DefaultSerializedTypes[underlyingType];
                    }
                }
                else if (underlyingNullableType != null)
                {
                    if (serializedType == SerializedType.Default)
                        serializedType = DefaultSerializedTypes[underlyingNullableType];
                }
                else if (serializedType == SerializedType.Default)
                {
                    serializedType = GetDefaultSerializedType(valueType, constLength);
                }

                var reader = new EndianAwareBinaryReader(stream, encoding, endianness);

                value = ReadPrimitive(reader, serializedType, encoding);
                value = value.ConvertTo(valueType);

                if (valueType.IsEnum)
                {
                    if (serializedType == SerializedType.NullTerminatedString)
                    {
                        var valueEnums = valueType.GetValuesForSerializedEnum();
                        value = valueEnums[value];
                    }

                    value = Enum.ToObject(valueType, value);
                }
            }
            else if (typeof (Stream).IsAssignableFrom(valueType))
            {
                /* This is weird but we need to find the base stream so we can reference it directly */
                var baseStream = stream;
                while (baseStream is StreamLimiter)
                    baseStream = (baseStream as StreamLimiter).Source;

                value = constLength == InvalidFieldLength ?
                    new Streamlet(baseStream) :
                    new Streamlet(baseStream, baseStream.Position, constLength);

                stream.Seek(constLength, SeekOrigin.Current);
            }
            else
            {
                value = Activator.CreateInstance(valueType);
                ReadMembers(stream, value, valueType, ctx);
            }

            return value;
        }

        private static object ReadPrimitive(EndianAwareBinaryReader reader, SerializedType serializedType, Encoding encoding)
        {
            var length = (reader.BaseStream is StreamLimiter)
                             ? reader.BaseStream.Length
                             : InvalidFieldLength;

            object value;
            switch (serializedType)
            {
                case SerializedType.Int1:
                    value = reader.ReadSByte();
                    break;
                case SerializedType.UInt1:
                    value = reader.ReadByte();
                    break;
                case SerializedType.Int2:
                    value = reader.ReadInt16();
                    break;
                case SerializedType.UInt2:
                    value = reader.ReadUInt16();
                    break;
                case SerializedType.Int4:
                    value = reader.ReadInt32();
                    break;
                case SerializedType.UInt4:
                    value = reader.ReadUInt32();
                    break;
                case SerializedType.Int8:
                    value = reader.ReadInt64();
                    break;
                case SerializedType.UInt8:
                    value = reader.ReadUInt64();
                    break;
                case SerializedType.Float4:
                    value = reader.ReadSingle();
                    break;
                case SerializedType.Float8:
                    value = reader.ReadDouble();
                    break;
                case SerializedType.ByteArray:
                    {
                        value = reader.ReadBytes((int) length);
                        break;
                    }
                case SerializedType.NullTerminatedString:
                    {
                        byte[] data = ReadNullTerminatedString(reader).ToArray();
                        value = encoding.GetString(data);
                        break;
                    }
                case SerializedType.SizedString:
                    {
                        if (length == InvalidFieldLength)
                        {
                            throw new InvalidOperationException(
                                "Fields with serialized type SizedString must have a FieldLengthAttribute");
                        }
                        byte[] data = reader.ReadBytes((int) length);
                        value = TrimNulls(encoding.GetString(data));
                        break;
                    }
                case SerializedType.LengthPrefixedString:
                    value = reader.ReadString();
                    break;
                default:
                    throw new NotSupportedException();
            }
            return value;
        }

        private static SerializedType GetDefaultSerializedType(Type valueType, long length)
        {
            SerializedType defaultSerializedType = DefaultSerializedTypes[valueType];

            /* If a length has been specified, disregard null terminated string type */
            if (defaultSerializedType == SerializedType.NullTerminatedString && length != InvalidFieldLength)
                defaultSerializedType = SerializedType.SizedString;

            return defaultSerializedType;
        }

        private static bool IsSerializable(MemberSerializationInfo memberSerializationInfo, object o,
                                           BinarySerializationContext ctx)
        {
            SerializeWhenAttribute[] serializeWhenAttributes = memberSerializationInfo.SerializeWhenAttributes;

            if (serializeWhenAttributes.Length == 0)
                return true;

            return serializeWhenAttributes.Any(serializeWhenAttribute =>
            {
                object value = ctx.GetValue(o, serializeWhenAttribute.Binding);

                if (value == null)
                    return false;

                return value.Equals(serializeWhenAttribute.Value);
            });
        }
        
        private static IList GetList(MemberSerializationInfo memberSerializationInfo,
                                     object o, Type valueType, out Type itemValueType)
        {
            itemValueType = GetListItemType(valueType);
            return memberSerializationInfo.Member.GetValue(o) as IList;
        }

        private static IList GetList(MemberSerializationInfo memberSerializationInfo,
                                     object o, BinarySerializationContext ctx, Type valueType,
                                     out Type itemValueType, out int count)
        {
            var list = memberSerializationInfo.Member.GetValue(o) as IList;

            FieldCountAttribute fieldCountAttribute = memberSerializationInfo.FieldCountAttribute;

            count = GetCount(fieldCountAttribute, o, ctx);

            itemValueType = GetListItemType(valueType);

            if (list == null)
            {
                if (valueType.IsArray)
                {
                    list = Array.CreateInstance(itemValueType, count);
                    memberSerializationInfo.Member.SetValue(o, list);
                }
                else
                {
                    list = (IList) Activator.CreateInstance(valueType);
                    memberSerializationInfo.Member.SetValue(o, list);
                }
            }
            return list;
        }

        private static Type GetListItemType(Type listType)
        {
            if (listType.IsArray)
            {
                return listType.GetElementType();
            }

            if (listType.GetGenericArguments().Length > 1)
            {
                throw new NotSupportedException("Multiple generic arguments not supported");
            }

            return listType.GetGenericArguments().Single();
        }

        private static long GetLength(ILengthAttribute lengthAttribute, object o, BinarySerializationContext ctx)
        {
            if (lengthAttribute == null)
                return 0;

            var binding = lengthAttribute.Binding;

            if (binding.Path == null)
                return (long)lengthAttribute.ConstLength;

            object value = ctx.GetValue(o, binding);

            if (value == null)
                return InvalidFieldLength;

            return (long)Convert.ToUInt64(value);
        }

        private static void SetValue(MemberBinding binding, object o, BinarySerializationContext ctx, long value)
        {
            ctx.SetValue(o, binding, value);
        }

        private static void SetValue(MemberBinding binding, object o, BinarySerializationContext ctx, string value)
        {
            ctx.SetValue(o, binding, value);
        }

        private static int GetCount(FieldCountAttribute countAttribute, object o, BinarySerializationContext ctx)
        {
            if (countAttribute == null)
                return int.MaxValue;

            var binding = countAttribute.Binding;

            if (binding.Path == null)
                return countAttribute.ConstCount;

            object value = ctx.GetValue(o, binding);
            return Convert.ToInt32(value);
        }

        private static long GetOffset(FieldOffsetAttribute fieldOffsetAttribute, object o, BinarySerializationContext ctx)
        {
            if (fieldOffsetAttribute == null)
                return 0;

            var binding = fieldOffsetAttribute.Binding;

            if (binding.Path == null)
                return fieldOffsetAttribute.ConstOffset;

            object value = ctx.GetValue(o, binding);

            if (value == null)
                return InvalidFieldOffset;

            return Convert.ToInt64(value);
        }

        private static Type GetSubtype(SubtypeAttribute[] subtypeAttributes, Type supertype, object o,
                                       BinarySerializationContext ctx)
        {
            if (subtypeAttributes == null || subtypeAttributes.Length == 0)
                return supertype;

            var matchingSubtypes = subtypeAttributes.Where(subtypeAttribute =>
                {
                    if (subtypeAttribute.Value == null)
                        return false;

                    var binding = subtypeAttribute.Binding;

                    if (binding.Path == null)
                        return false;

                    object value = ctx.GetValue(o, binding);

                    if (value == null)
                        return false;

                    var comparissonType = value.ConvertTo(subtypeAttribute.Value.GetType());
                    return comparissonType.Equals(subtypeAttribute.Value);
                }).ToList();

            if (matchingSubtypes.Count == 0)
                return null;

            if(matchingSubtypes.Count > 1)
                throw new ArgumentException("Subtypes must have unique values.", "subtypeAttributes");

            return matchingSubtypes[0].Subtype;
        }

        
        private static void SetSubtype(SubtypeAttribute[] subtypeAttributes, Type type, object o,
                                       BinarySerializationContext ctx)
        {
            if (subtypeAttributes == null)
                throw new ArgumentNullException("subtypeAttributes");

            if(subtypeAttributes.Length == 0)
                throw new ArgumentException("cannot be empty.", "subtypeAttributes");

            var matchingSubtypes = subtypeAttributes.Where(a => a.Subtype == type).ToList();

            if (matchingSubtypes.Count == 0)
            {
                /* Try to fall back on base types */
                matchingSubtypes = subtypeAttributes.Where(a => a.Subtype.IsAssignableFrom(type)).ToList();

                if (matchingSubtypes.Count == 0)
                    throw new InvalidOperationException(
                        string.Format("No matching subtype attribute for type '{0}'.", type));
            }

            if (matchingSubtypes.Count > 1)
                throw new ArgumentException("Subtypes must have unique values.", "subtypeAttributes");

            var subtypeAttribute = matchingSubtypes[0];

            if (subtypeAttribute.Value is string)
            {
                SetValue(subtypeAttribute.Binding, o, ctx, subtypeAttribute.Value as string);
            }
            else
            {
                var value = Convert.ToInt64(subtypeAttribute.Value);
                SetValue(subtypeAttribute.Binding, o, ctx, value);
            }
        }


        private static bool ShouldTreatAsList(Type type)
        {
            /* Exclude special cases of byte arrays and nulls */
            return type != null && type.IsList() && !type.IsByteArray();
        }

        private static IEnumerable<byte> ReadNullTerminatedString(BinaryReader reader)
        {
            byte b;
            while ((b = reader.ReadByte()) != 0)
                yield return b;
        }

        private static string TrimNulls(string s)
        {
            return s.TrimEnd('\0');
        }

        private void OnMemberSerialized(string memberName, object value, BinarySerializationContext context)
        {
            if (MemberSerialized != null)
                MemberSerialized(this, new MemberSerializedEventArgs(memberName, value, context));
        }

        private void OnMemberDeserialized(string memberName, object value, BinarySerializationContext context)
        {
            if (MemberDeserialized != null)
                MemberDeserialized(this, new MemberSerializedEventArgs(memberName, value, context));
        }

        private void OnMemberSerializing(string memberName, BinarySerializationContext context)
        {
            if(MemberSerializing != null)
                MemberSerializing(this, new MemberSerializingEventArgs(memberName, context));
        }

        private void OnMemberDeserializing(string memberName, BinarySerializationContext context)
        {
            if(MemberDeserializing != null)
                MemberDeserializing(this, new MemberSerializingEventArgs(memberName, context));
        }
    }
}