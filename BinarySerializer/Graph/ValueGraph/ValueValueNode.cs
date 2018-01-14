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

                        if (targetValues.Any(v => !value.Equals(v)))
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

        public void Serialize(BoundedStream stream, object value, SerializedType serializedType, long? length = null)
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

                    var data = new byte[constLength.Value];

                    if (serializedType == SerializedType.TerminatedString && data.Length > 0)
                    {
                        data[0] = TypeNode.StringTerminator;
                    }

                    stream.Write(data, 0, (int) constLength.Value);

                    return;
                }
            }

            var maxLength = GetMaxLength(stream, serializedType);

            var scaledValue = ScaleValue(value);
            var convertedValue = GetValue(scaledValue, serializedType);

            switch (serializedType)
            {
                case SerializedType.Int1:
                {
                    stream.WriteByte((byte) Convert.ToSByte(convertedValue));
                    break;
                }
                case SerializedType.UInt1:
                {
                    stream.WriteByte(Convert.ToByte(convertedValue));
                    break;
                }
                case SerializedType.Int2:
                {
                    var data = BitConverter.GetBytes(Convert.ToInt16(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.UInt2:
                {
                    var data = BitConverter.GetBytes(Convert.ToUInt16(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.Int4:
                {
                    var data = BitConverter.GetBytes(Convert.ToInt32(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.UInt4:
                {
                    var data = BitConverter.GetBytes(Convert.ToUInt32(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.Int8:
                {
                    var data = BitConverter.GetBytes(Convert.ToInt64(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.UInt8:
                {
                    var data = BitConverter.GetBytes(Convert.ToUInt64(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.Float4:
                {
                    var data = BitConverter.GetBytes(Convert.ToSingle(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.Float8:
                {
                    var data = BitConverter.GetBytes(Convert.ToDouble(convertedValue));
                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.ByteArray:
                {
                    var data = (byte[]) value;

                    AdjustArrayLength(ref data, constLength, maxLength);

                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());

                    AdjustNullTerminatedArrayLength(ref data, constLength, maxLength);

                    stream.Write(data, 0, data.Length);
                    stream.WriteByte(TypeNode.StringTerminator);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());

                    AdjustArrayLength(ref data, constLength, maxLength);

                    stream.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    if (constLength != null)
                    {
                        throw new NotSupportedException("Length-prefixed strings cannot have a const length.");
                    }

                    var writer = new BinaryWriter(stream, GetFieldEncoding());
                    writer.Write(value.ToString());
                    break;
                }

                default:
                    throw new NotSupportedException();
            }
        }

        public Task SerializeAsync(BoundedStream stream, object value, SerializedType serializedType,
            long? length = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writer = new AsyncBinaryWriter(stream, GetFieldEncoding());
            return SerializeAsync(writer, value, serializedType, length, cancellationToken);
        }

        public async Task SerializeAsync(AsyncBinaryWriter writer, object value, SerializedType serializedType,
            long? length = null, CancellationToken cancellationToken = default(CancellationToken))
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

                    var data = new byte[constLength.Value];

                    if (serializedType == SerializedType.TerminatedString && data.Length > 0)
                    {
                        data[0] = TypeNode.StringTerminator;
                    }

                    await writer.WriteAsync(data, (int) constLength.Value, cancellationToken).ConfigureAwait(false);

                    return;
                }
            }

            var maxLength = GetMaxLength((BoundedStream) writer.BaseStream, serializedType);

            var scaledValue = ScaleValue(value);
            var convertedValue = GetValue(scaledValue, serializedType);

            switch (serializedType)
            {
                case SerializedType.Int1:
                {
                    await writer.WriteAsync(Convert.ToSByte(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt1:
                {
                    await writer.WriteAsync(Convert.ToByte(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Int2:
                {
                    await writer.WriteAsync(Convert.ToInt16(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt2:
                {
                    await writer.WriteAsync(Convert.ToUInt16(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Int4:
                {
                    await writer.WriteAsync(Convert.ToInt32(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt4:
                {
                    await writer.WriteAsync(Convert.ToUInt32(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Int8:
                {
                    await writer.WriteAsync(Convert.ToInt64(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.UInt8:
                {
                    await writer.WriteAsync(Convert.ToUInt64(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Float4:
                {
                    await writer.WriteAsync(Convert.ToSingle(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.Float8:
                {
                    await writer.WriteAsync(Convert.ToDouble(convertedValue), cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.ByteArray:
                {
                    var data = (byte[]) value;

                    AdjustArrayLength(ref data, constLength, maxLength);

                    await writer.WriteAsync(data, data.Length, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());

                    AdjustNullTerminatedArrayLength(ref data, constLength, maxLength);

                    await writer.WriteAsync(data, data.Length, cancellationToken).ConfigureAwait(false);

                    var stringTerminatorData = new[] {TypeNode.StringTerminator};
                    await writer.WriteAsync(stringTerminatorData, stringTerminatorData.Length, cancellationToken)
                        .ConfigureAwait(false);

                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = GetFieldEncoding().GetBytes(value.ToString());

                    AdjustArrayLength(ref data, constLength, maxLength);

                    await writer.WriteAsync(data, data.Length, cancellationToken).ConfigureAwait(false);
                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    if (constLength != null)
                    {
                        throw new NotSupportedException("Length-prefixed strings cannot have a const length.");
                    }

                    writer.Write(value.ToString());
                    break;
                }

                default:
                    throw new NotSupportedException();
            }
        }

        public void Deserialize(BoundedStream stream, SerializedType serializedType, long? length = null)
        {
            var reader = new BinaryReader(stream);
            Deserialize(reader, serializedType, length);
        }

        public Task DeserializeAsync(BoundedStream stream, SerializedType serializedType, long? length,
            CancellationToken cancellationToken)
        {
            var reader = new AsyncBinaryReader(stream);
            return DeserializeAsync(reader, serializedType, length, cancellationToken);
        }

        public void Deserialize(BinaryReader reader, SerializedType serializedType, long? length = null)
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
                    value = reader.ReadBytes((int) effectiveLengthValue);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = ReadTerminated(reader, (int) effectiveLengthValue, TypeNode.StringTerminator);
                    value = GetFieldEncoding().GetString(data, 0, data.Length);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = reader.ReadBytes((int) effectiveLengthValue);
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

        public async Task DeserializeAsync(AsyncBinaryReader reader, SerializedType serializedType, long? length,
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
                    value = await reader.ReadBytesAsync((int) effectiveLengthValue, cancellationToken)
                        .ConfigureAwait(false);
                    break;
                }
                case SerializedType.TerminatedString:
                {
                    var data = await ReadTerminatedAsync(reader, (int) effectiveLengthValue, TypeNode.StringTerminator,
                            cancellationToken)
                        .ConfigureAwait(false);
                    value = GetFieldEncoding().GetString(data, 0, data.Length);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var data = await reader.ReadBytesAsync((int) effectiveLengthValue, cancellationToken)
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
                    return Task.CompletedTask;
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

        protected override long MeasureOverride()
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

        private long? GetConstLength(long? length)
        {
            // prioritization of field length specifiers
            var constLength = length ?? // explicit length passed from parent
                              GetConstFieldLength() ?? // calculated length from this field
                              GetConstFieldCount(); // explicit field count (used for byte arrays and strings)
            return constLength;
        }

        private static long? GetMaxLength(BoundedStream stream, SerializedType serializedType)
        {
            long? maxLength = null;

            if (serializedType == SerializedType.ByteArray || serializedType == SerializedType.SizedString ||
                serializedType == SerializedType.TerminatedString)
            {
                // try to get bounded length
                maxLength = stream.AvailableForWriting;
            }
            return maxLength;
        }

        private static void AdjustArrayLength(ref byte[] data, long? constLength, long? maxLength)
        {
            if (constLength != null)
            {
                Array.Resize(ref data, (int) constLength.Value);
            }

            if (maxLength != null && data.Length > maxLength)
            {
                Array.Resize(ref data, (int) maxLength.Value);
            }
        }

        private static void AdjustNullTerminatedArrayLength(ref byte[] data, long? constLength, long? maxLength)
        {
            if (constLength != null)
            {
                Array.Resize(ref data, (int) constLength.Value - 1);
            }

            if (maxLength != null && data.Length > maxLength)
            {
                Array.Resize(ref data, (int) maxLength.Value - 1);
            }
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

        private long GetEffectiveLengthValue(BinaryReader reader, SerializedType serializedType, long? length)
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

                    checked
                    {
                        effectiveLength = (int) baseStream.AvailableForReading;
                    }
                }
            }

            var effectiveLengthValue = effectiveLength ?? 0;
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

        private static byte[] ReadTerminated(BinaryReader reader, int maxLength, byte terminator)
        {
            var buffer = new MemoryStream();

            byte b;
            while (maxLength-- > 0 && (b = reader.ReadByte()) != terminator)
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