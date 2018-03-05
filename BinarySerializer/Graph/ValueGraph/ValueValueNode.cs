using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ValueValueNode : ValueNode
    {
        private object _cachedValue;
        private object _value;

        private const string LengthPrefixedStringsCannotHaveConstLengthMessage =
            "Length-prefixed strings cannot have a const length.";


        public ValueValueNode(ValueNode parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get => GetValue(TypeNode.GetSerializedType());
            set => _cachedValue = value;
        }

        public override object BoundValue
        {
            get
            {
                object value;

                if (Bindings.Count == 0)
                {
                    value = Value;
                }
                else
                {
                    value = Bindings[0].Invoke();

                    if (value == UnsetValue)
                    {
                        value = Value;
                    }

                    if (Bindings.Count != 1)
                    {
                        var targetValues = Bindings.Select(binding =>
                            {
                                var bindingValue = binding();
                                return bindingValue == UnsetValue ? Value : bindingValue;
                            })
                            .ToArray();

                        if (targetValues.Any(v => !Equals(value, v)))
                        {
                            throw new BindingException(
                                "Multiple bindings to a single source must have equivalent target values.");
                        }
                    }

                    // handle case where we might be binding to a list or array
                    if (value is IEnumerable enumerableValue)
                    {
                        // handle special cases
                        if (TypeNode.Type == typeof(byte[]) || TypeNode.Type == typeof(string))
                        {
                            var data = enumerableValue.Cast<object>().Select(Convert.ToByte).ToArray();

                            if (TypeNode.Type == typeof(byte[]))
                            {
                                value = data;
                            }
                            else if (TypeNode.Type == typeof(string))
                            {
                                value = GetFieldEncoding().GetString(data, 0, data.Length);
                            }
                        }
                        else
                        {
                            value = GetScalar(enumerableValue);
                        }
                    }
                }

                return ConvertToFieldType(value);
            }
        }

        public object GetValue(SerializedType serializedType)
        {
            if (_cachedValue != null)
            {
                return _cachedValue;
            }

            return GetValue(_value, serializedType);
        }


        public void Serialize(BoundedStream stream, object value, SerializedType serializedType,
            FieldLength length = null)
        {
            var writer = new AsyncBinaryWriter(stream, GetFieldEncoding());
            Serialize(writer, value, serializedType, length);
        }

        public void Serialize(AsyncBinaryWriter writer, object value, SerializedType serializedType, FieldLength length = null)
        {
            var constLength = GetConstLength(length);

            if (value == null)
            {
                /* In the special case of sized strings, don't allow nulls */
                if (serializedType == SerializedType.SizedString)
                {
                    value = string.Empty;
                }
                else
                {
                    // see if we're dealing with a const length field
                    if (constLength == null)
                    {
                        return;
                    }

                    // see if we're dealing with something that needs to be padded
                    if (serializedType != SerializedType.ByteArray &&
                        serializedType != SerializedType.TerminatedString)
                    {
                        return;
                    }

                    var data = new byte[constLength.TotalByteCount];

                    if (serializedType == SerializedType.TerminatedString && data.Length > 0)
                    {
                        data[0] = TypeNode.StringTerminator;
                    }

                    writer.Write(data, constLength);

                    return;
                }
            }

            var outputStream = writer.OutputStream;
            var maxLength = GetMaxLength(outputStream, serializedType);

            var scaledValue = ScaleValue(value);
            var convertedValue = GetValue(scaledValue, serializedType);

            switch (serializedType)
            {
                case SerializedType.Int1:
                    {
                        writer.Write(Convert.ToSByte(convertedValue), constLength);
                        break;
                    }
                case SerializedType.UInt1:
                    {
                        writer.Write(Convert.ToByte(convertedValue), constLength);
                        break;
                    }
                case SerializedType.Int2:
                    {
                        writer.Write(Convert.ToInt16(convertedValue), constLength);
                        break;
                    }
                case SerializedType.UInt2:
                    {
                        writer.Write(Convert.ToUInt16(convertedValue), constLength);
                        break;
                    }
                case SerializedType.Int4:
                    {
                        writer.Write(Convert.ToInt32(convertedValue), constLength);
                        break;
                    }
                case SerializedType.UInt4:
                    {
                        writer.Write(Convert.ToUInt32(convertedValue), constLength);
                        break;
                    }
                case SerializedType.Int8:
                    {
                        writer.Write(Convert.ToInt64(convertedValue), constLength);
                        break;
                    }
                case SerializedType.UInt8:
                    {
                        writer.Write(Convert.ToUInt64(convertedValue), constLength);
                        break;
                    }
                case SerializedType.Float4:
                    {
                        writer.Write(Convert.ToSingle(convertedValue), constLength);
                        break;
                    }
                case SerializedType.Float8:
                    {
                        writer.Write(Convert.ToDouble(convertedValue), constLength);
                        break;
                    }
                case SerializedType.ByteArray:
                {
                    var data = (byte[]) value;

                    var dataLength = GetArrayLength(data, constLength, maxLength);

                    writer.Write(data, dataLength);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());

                    var dataLength = GetNullTerminatedArrayLength(data, constLength, maxLength);

                    writer.Write(data, dataLength);
                    var stringTerminatorData = new[] {TypeNode.StringTerminator};
                    writer.Write(stringTerminatorData, stringTerminatorData.Length);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());
                        
                    var dataLength = GetArrayLength(data, constLength, maxLength);

                    writer.Write(data, dataLength);
                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    if (constLength != null)
                    {
                        throw new NotSupportedException(LengthPrefixedStringsCannotHaveConstLengthMessage);
                    }

                    writer.Write(value.ToString());
                        break;
                }

                default:
                    throw new NotSupportedException();
            }
        }

        public Task SerializeAsync(BoundedStream stream, object value, SerializedType serializedType,
            FieldLength length = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writer = new AsyncBinaryWriter(stream, GetFieldEncoding());
            return SerializeAsync(writer, value, serializedType, length, cancellationToken);
        }

        public async Task SerializeAsync(AsyncBinaryWriter writer, object value, SerializedType serializedType,
            FieldLength length = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var constLength = GetConstLength(length);

            if (value == null)
            {
                /* In the special case of sized strings, don't allow nulls */
                if (serializedType == SerializedType.SizedString)
                {
                    value = string.Empty;
                }
                else
                {
                    // see if we're dealing with a const length field
                    if (constLength == null)
                    {
                        return;
                    }

                    // see if we're dealing with something that needs to be padded
                    if (serializedType != SerializedType.ByteArray &&
                        serializedType != SerializedType.TerminatedString)
                    {
                        return;
                    }

                    var data = new byte[constLength.TotalByteCount];

                    if (serializedType == SerializedType.TerminatedString && data.Length > 0)
                    {
                        data[0] = TypeNode.StringTerminator;
                    }

                    await writer.WriteAsync(data, constLength, cancellationToken).ConfigureAwait(false);

                    return;
                }
            }

            var outputStream = writer.OutputStream;
            var maxLength = GetMaxLength(outputStream, serializedType);

            var scaledValue = ScaleValue(value);
            var convertedValue = GetValue(scaledValue, serializedType);
            
            switch (serializedType)
            {
                case SerializedType.Int1:
                {
                    await writer.WriteAsync(Convert.ToSByte(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt1:
                {
                    await writer.WriteAsync(Convert.ToByte(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Int2:
                {
                    await writer.WriteAsync(Convert.ToInt16(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt2:
                {
                    await writer.WriteAsync(Convert.ToUInt16(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Int4:
                {
                    await writer.WriteAsync(Convert.ToInt32(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt4:
                {
                    await writer.WriteAsync(Convert.ToUInt32(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Int8:
                {
                    await writer.WriteAsync(Convert.ToInt64(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt8:
                {
                    await writer.WriteAsync(Convert.ToUInt64(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Float4:
                {
                    await writer.WriteAsync(Convert.ToSingle(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Float8:
                {
                    await writer.WriteAsync(Convert.ToDouble(convertedValue), constLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.ByteArray:
                {
                    var data = (byte[]) value;

                    var dataLength = GetArrayLength(data, constLength, maxLength);

                    await writer.WriteAsync(data, dataLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());
                        
                    var dataLength = GetNullTerminatedArrayLength(data, constLength, maxLength);

                    await writer.WriteAsync(data, dataLength, cancellationToken).ConfigureAwait(false);

                    var stringTerminatorData = new[] {TypeNode.StringTerminator};
                    await writer.WriteAsync(stringTerminatorData, stringTerminatorData.Length, cancellationToken)
                        .ConfigureAwait(false);

                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());
                    var dataLength = GetArrayLength(data, constLength, maxLength);

                    await writer.WriteAsync(data, dataLength, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    if (constLength != null)
                    {
                        throw new NotSupportedException(LengthPrefixedStringsCannotHaveConstLengthMessage);
                    }

                    writer.Write(value.ToString());
                    break;
                }

                default:
                    throw new NotSupportedException();
            }
        }

        public void Deserialize(BoundedStream stream, SerializedType serializedType, FieldLength length = null)
        {
            var reader = new AsyncBinaryReader(stream, GetFieldEncoding());
            Deserialize(reader, serializedType, length);
        }

        public Task DeserializeAsync(BoundedStream stream, SerializedType serializedType, FieldLength length,
            CancellationToken cancellationToken)
        {
            var reader = new AsyncBinaryReader(stream, GetFieldEncoding());
            return DeserializeAsync(reader, serializedType, length, cancellationToken);
        }

        public void Deserialize(BinaryReader reader, SerializedType serializedType, FieldLength length = null)
        {
            var effectiveLengthValue = GetEffectiveLengthValue(reader, serializedType, length);

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
                    value = reader.ReadBytes((int) effectiveLengthValue.ByteCount);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = ReadTerminated(reader, effectiveLengthValue, TypeNode.StringTerminator);
                    value = GetFieldEncoding().GetString(data, 0, data.Length);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = reader.ReadBytes((int) effectiveLengthValue.ByteCount);
                    value = GetString(data);

                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    value = reader.ReadString();
                    break;
                }

                default:
                    throw new NotSupportedException();
            }

            var convertedValue = ConvertToFieldType(value);
            _value = UnscaleValue(convertedValue);

            // check computed values (CRCs, etc.)
            CheckComputedValues();
        }

        public async Task DeserializeAsync(AsyncBinaryReader reader, SerializedType serializedType, FieldLength length,
            CancellationToken cancellationToken)
        {
            var effectiveLengthValue = GetEffectiveLengthValue(reader, serializedType, length);

            object value;
            switch (serializedType)
            {
                case SerializedType.Int1:
                    value = await reader.ReadSByteAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.UInt1:
                    value = await reader.ReadByteAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.Int2:
                    value = await reader.ReadInt16Async(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.UInt2:
                    value = await reader.ReadUInt16Async(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.Int4:
                    value = await reader.ReadInt32Async(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.UInt4:
                    value = await reader.ReadUInt32Async(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.Int8:
                    value = await reader.ReadInt64Async(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.UInt8:
                    value = await reader.ReadUInt64Async(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.Float4:
                    value = await reader.ReadSingleAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.Float8:
                    value = await reader.ReadDoubleAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case SerializedType.ByteArray:
                {
                    value = await reader.ReadBytesAsync((int) effectiveLengthValue.ByteCount, cancellationToken)
                        .ConfigureAwait(false);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = await ReadTerminatedAsync(reader, (int) effectiveLengthValue.ByteCount, TypeNode.StringTerminator,
                            cancellationToken)
                        .ConfigureAwait(false);
                    value = GetFieldEncoding().GetString(data, 0, data.Length);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = await reader.ReadBytesAsync((int) effectiveLengthValue.ByteCount, cancellationToken)
                        .ConfigureAwait(false);
                    value = GetString(data);

                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    value = reader.ReadString();
                    break;
                }

                default:
                    throw new NotSupportedException();
            }

            var convertedValue = ConvertToFieldType(value);
            _value = UnscaleValue(convertedValue);

            // check computed values (CRCs, etc.)
            CheckComputedValues();
        }

        public override string ToString()
        {
            if (Name != null)
            {
                return $"{Name}: {Value}";
            }

            return Value?.ToString() ?? base.ToString();
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            Serialize(stream, BoundValue, TypeNode.GetSerializedType());
        }

        internal override Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            return SerializeAsync(stream, BoundValue, TypeNode.GetSerializedType(), null, cancellationToken);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            if (EndOfStream(stream))
            {
                if (TypeNode.IsNullable)
                {
                    return;
                }

                throw new EndOfStreamException();
            }

            Deserialize(stream, TypeNode.GetSerializedType());
        }

        internal override Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            if (EndOfStream(stream))
            {
                if (TypeNode.IsNullable)
                {
                    #if NETSTANDARD1_3
                    return Task.CompletedTask;
                    #else
                    return Task.FromResult(true);
                    #endif
                }

                throw new EndOfStreamException();
            }

            return DeserializeAsync(stream, TypeNode.GetSerializedType(), null, cancellationToken);
        }

        protected override long CountOverride()
        {
            // handle special case of byte[]
            var boundValue = BoundValue as byte[];
            return boundValue?.Length ?? base.CountOverride();
        }

        protected override FieldLength MeasureOverride()
        {
            // handle special case of byte[]
            var boundValue = BoundValue as byte[];
            return boundValue?.Length ?? base.MeasureOverride();
        }

        private object ScaleValue(object value)
        {
            if (TypeNode.FieldScaleBindings == null)
            {
                return value;
            }

            var scale = TypeNode.FieldScaleBindings.GetValue(this);

            return Convert.ToDouble(value) * Convert.ToDouble(scale);
        }

        private object UnscaleValue(object value)
        {
            if (TypeNode.FieldScaleBindings == null)
            {
                return value;
            }

            var scale = TypeNode.FieldScaleBindings.GetValue(this);

            return Convert.ToDouble(value) / Convert.ToDouble(scale);
        }

        private FieldLength GetConstLength(FieldLength length)
        {
            // prioritization of field length specifiers
            var constLength = length ?? // explicit length passed from parent
                              GetConstFieldLength() ?? // calculated length from this field
                              GetConstFieldCount(); // explicit field count (used for byte arrays and strings)

            return constLength;
        }

        private static FieldLength GetMaxLength(BoundedStream stream, SerializedType serializedType)
        {
            FieldLength maxLength = null;

            if (serializedType == SerializedType.ByteArray || serializedType == SerializedType.SizedString ||
                serializedType == SerializedType.TerminatedString)
            {
                // try to get bounded length
                maxLength = stream.AvailableForWriting;
            }

            return maxLength;
        }

        private static FieldLength GetArrayLength(byte[] data, FieldLength constLength, FieldLength maxLength)
        {
            FieldLength length = data.Length;

            if (constLength != null)
            {
                length = constLength;
            }

            if (maxLength != null && length > maxLength)
            {
                length = maxLength;
            }

            return length;
        }

        private static FieldLength GetNullTerminatedArrayLength(byte[] data, FieldLength constLength, FieldLength maxLength)
        {
            FieldLength length = data.Length;

            if (constLength != null)
            {
                length = constLength - new FieldLength(1);
            }

            if (maxLength != null && length > maxLength)
            {
                length = maxLength - new FieldLength(1);
            }

            return length;
        }

        private object GetScalar(IEnumerable enumerable)
        {
            var childLengths = enumerable.Cast<object>().Select(ConvertToFieldType).ToList();

            if (childLengths.Count == 0)
            {
                return 0;
            }

            var childLengthGroups = childLengths.GroupBy(childLength => childLength).ToList();

            if (childLengthGroups.Count > 1)
            {
                throw new InvalidOperationException(
                    "Unable to update scalar binding source because not all enumerable items have equal lengths.");
            }

            var childLengthGroup = childLengthGroups.Single();

            return childLengthGroup.Key;
        }

        private object GetString(byte[] data)
        {
            object value;
            var untrimmed = GetFieldEncoding().GetString(data, 0, data.Length);
            if (TypeNode.AreStringsTerminated)
            {
                var terminatorIndex = untrimmed.IndexOf((char) TypeNode.StringTerminator);
                value = terminatorIndex != -1 ? untrimmed.Substring(0, terminatorIndex) : untrimmed;
            }
            else
            {
                value = untrimmed;
            }
            return value;
        }

        private FieldLength GetEffectiveLengthValue(BinaryReader reader, SerializedType serializedType, FieldLength length)
        {
            var effectiveLength = length ?? GetFieldLength() ?? GetFieldCount();

            if (effectiveLength == null)
            {
                if (serializedType == SerializedType.ByteArray ||
                    serializedType == SerializedType.SizedString ||
                    serializedType == SerializedType.TerminatedString)
                {
                    // try to get bounded length
                    var baseStream = (BoundedStream) reader.BaseStream;

                    effectiveLength = baseStream.AvailableForReading;
                }
            }

            var effectiveLengthValue = effectiveLength ?? FieldLength.Zero;
            return effectiveLengthValue;
        }

        private object GetValue(object value, SerializedType serializedType)
        {
            if (value == null)
            {
                return null;
            }

            // only resolve endianness if it matters
            if (serializedType == SerializedType.Int2 || serializedType == SerializedType.UInt2 ||
                serializedType == SerializedType.Int4 || serializedType == SerializedType.UInt4 ||
                serializedType == SerializedType.Int8 || serializedType == SerializedType.UInt8 ||
                serializedType == SerializedType.Float4 || serializedType == SerializedType.Float8)
            {
                if (GetFieldEndianness() != Endianness.Big)
                {
                    return value;
                }

                switch (serializedType)
                {
                    case SerializedType.Int2:
                        return Bytes.Reverse(Convert.ToInt16(value));
                    case SerializedType.UInt2:
                        var value2 = Bytes.Reverse(Convert.ToUInt16(value));

                        // handle special case of char
                        return ConvertToFieldType(value2);
                    case SerializedType.Int4:
                        return Bytes.Reverse(Convert.ToInt32(value));
                    case SerializedType.UInt4:
                        return Bytes.Reverse(Convert.ToUInt32(value));
                    case SerializedType.Int8:
                        return Bytes.Reverse(Convert.ToInt64(value));
                    case SerializedType.UInt8:
                        return Bytes.Reverse(Convert.ToUInt64(value));
                    case SerializedType.Float4:
                        return Bytes.Reverse(Convert.ToSingle(value));
                    case SerializedType.Float8:
                        return Bytes.Reverse(Convert.ToDouble(value));
                }
            }

            return value;
        }

        private object ConvertToFieldType(object value)
        {
            return value.ConvertTo(TypeNode.Type);
        }

        private void CheckComputedValues()
        {
            bool isMatch = true;
            string expected = null;
            string actual = null;

            var value = Value;
            var boundValue = BoundValue;

            if (boundValue != null && Bindings.Count > 0)
            {
                // check special case of arrays
                if (boundValue is byte[] boundValueArray && value is byte[] valueArray)
                {
                    if (!boundValueArray.SequenceEqual(valueArray))
                    {
                        expected = BitConverter.ToString(boundValueArray);
                        actual = BitConverter.ToString(valueArray);
                        isMatch = false;
                    }
                }
                else if (!boundValue.Equals(value))
                {
                    expected = boundValue.ToString();
                    actual = value.ToString();
                    isMatch = false;
                }
            }

            if (!isMatch)
            {
                throw new InvalidDataException($"Deserialized value does not match computed value.  Expected {expected} but got {actual}.  To suppress this check, change binding mode to OneWayToSource.");
            }
        }

        private static byte[] ReadTerminated(BinaryReader reader, FieldLength maxLength, byte terminator)
        {
            var buffer = new MemoryStream();

            var byteLength = maxLength.ByteCount;

            byte b;
            while (byteLength-- > 0 && (b = reader.ReadByte()) != terminator)
            {
                buffer.WriteByte(b);
            }

            return buffer.ToArray();
        }

        private static async Task<byte[]> ReadTerminatedAsync(AsyncBinaryReader reader, int maxLength, byte terminator,
            CancellationToken cancellationToken)
        {
            var buffer = new MemoryStream();

            byte b;
            while (maxLength-- > 0 &&
                   (b = await reader.ReadByteAsync(cancellationToken).ConfigureAwait(false)) != terminator)
            {
                buffer.WriteByte(b);
            }

            return buffer.ToArray();
        }
    }
}