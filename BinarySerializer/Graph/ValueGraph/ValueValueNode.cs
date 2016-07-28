using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ValueValueNode : ValueNode
    {
        public ValueValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

        public override object Value { get; set; }

        public override object BoundValue
        {
            get
            {
                object value;

                if (Bindings.Count > 0)
                {
                    value = Bindings[0].Invoke();

                    if (Bindings.Count != 1)
                    {
                        object[] targetValues = Bindings.Select(binding => binding()).ToArray();

                        if (targetValues.Any(v => !value.Equals(v)))
                            throw new BindingException(
                                "Multiple bindings to a single source must have equivalent target values.");
                    }

                    // handle case where we might be binding to a list or array
                    var enumerableValue = value as IEnumerable;
                    
                    if (enumerableValue != null)
                    {
                        // handle special cases
                        if (TypeNode.Type == typeof (byte[]) || TypeNode.Type == typeof (string))
                        {
                            var data = enumerableValue.Cast<object>().Select(Convert.ToByte).ToArray();

                            if (TypeNode.Type == typeof (byte[]))
                                value = data;
                            else if (TypeNode.Type == typeof (string))
                                value = GetFieldEncoding().GetString(data, 0, data.Length);
                        }
                        else value = GetScalar(enumerableValue);
                    }
                }
                else value = Value;

                return ConvertToFieldType(value);
            }
        }

        private object GetScalar(IEnumerable enumerable)
        {
            var childLengths = enumerable.Cast<object>().Select(ConvertToFieldType).ToList();

            if (childLengths.Count == 0)
                return 0;

            var childLengthGroups = childLengths.GroupBy(childLength => childLength).ToList();

            if (childLengthGroups.Count > 1)
                throw new InvalidOperationException("Unable to update scalar binding source because not all enumerable items have equal lengths.");

            var childLengthGroup = childLengthGroups.Single();

            return childLengthGroup.Key;
        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            Serialize(stream, BoundValue, TypeNode.GetSerializedType());
        }

        public void Serialize(Stream stream, object value, SerializedType serializedType, long? length = null)
        {
            var writer = new EndianAwareBinaryWriter(stream, GetFieldEndianness());
            Serialize(writer, value, serializedType, length);
        }

        public void Serialize(EndianAwareBinaryWriter writer, object value, SerializedType serializedType,
            long? length = null)
        {
            if (value == null)
            {
                /* In the special case of sized strings, don't allow nulls */
                if (serializedType == SerializedType.SizedString)
                    value = string.Empty;
                else return;
            }

            // prioritization of field length specifiers
            long? constLength = length ?? // explicit length passed from parent
                                GetConstFieldLength() ?? // calculated length from this field
                                GetConstFieldCount(); // explicit field count (used for byte arrays and strings)
            
            long? maxLength = null;

            if (serializedType == SerializedType.ByteArray || serializedType == SerializedType.SizedString || serializedType == SerializedType.NullTerminatedString)
            {
                // try to get bounded length
                var baseStream = (BoundedStream) writer.BaseStream;
                maxLength = baseStream.AvailableForWriting;
            }

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
                    writer.Write(data, 0, data.Length);
                    break;
                }
                case SerializedType.NullTerminatedString:
                {
                    byte[] data = GetFieldEncoding().GetBytes(value.ToString());

                    if (constLength != null)
                        Array.Resize(ref data, (int)constLength.Value - 1);

                    if(maxLength != null && data.Length > maxLength)
                        Array.Resize(ref data, (int)maxLength.Value - 1);

                    writer.Write(data);
                    writer.Write((byte) 0);
                    break;
                }
                case SerializedType.SizedString:
                {
                    byte[] data = GetFieldEncoding().GetBytes(value.ToString());

                    if (constLength != null)
                        Array.Resize(ref data, (int)constLength.Value);

                    if (maxLength != null && data.Length > maxLength)
                        Array.Resize(ref data, (int)maxLength.Value);

                    writer.Write(data);

                    break;
                }
                case SerializedType.LengthPrefixedString:
                {
                    if(constLength != null)
                        throw new NotSupportedException("Length-prefixed strings cannot have a const length.");

                    writer.Write(value.ToString());
                    break;
                }

                default:
                    throw new NotSupportedException();
            }
        }



        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            if (EndOfStream(stream))
            {
                if (TypeNode.IsNullable)
                    return;

                throw new EndOfStreamException();
            }

            object value = Deserialize(stream, TypeNode.GetSerializedType());
            Value = ConvertToFieldType(value);
        }

        public object Deserialize(BoundedStream stream, SerializedType serializedType, long? length = null)
        {
            var reader = new EndianAwareBinaryReader(stream, GetFieldEndianness());
            return Deserialize(reader, serializedType, length);
        }

        public object Deserialize(EndianAwareBinaryReader reader, SerializedType serializedType, long? length = null)
        {
            long? effectiveLength = length ?? GetFieldLength() ?? GetFieldCount();

            if (effectiveLength == null)
            {
                if (serializedType == SerializedType.ByteArray || serializedType == SerializedType.SizedString ||
                         serializedType == SerializedType.NullTerminatedString)
                {
                    // try to get bounded length
                    var baseStream = (BoundedStream) reader.BaseStream;

                    checked
                    {
                        effectiveLength = (int) baseStream.AvailableForReading;
                    }
                }
            }

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
                    value = reader.ReadBytes((int)effectiveLength.Value);
                    break;
                }
                case SerializedType.NullTerminatedString:
                {
                    byte[] data = ReadNullTerminated(reader, (int)effectiveLength.Value).ToArray();

                    value = GetFieldEncoding().GetString(data, 0, data.Length);
                    break;
                }
                case SerializedType.SizedString:
                {
                    byte[] data = reader.ReadBytes((int)effectiveLength.Value);
                    var untrimmed = GetFieldEncoding().GetString(data, 0, data.Length);
                    if (TypeNode.AreStringsNullTerminated)
                    {
                        var nullIndex = untrimmed.IndexOf((char) 0);
                        value = nullIndex != -1 ? untrimmed.Substring(0, nullIndex) : untrimmed;
                    }
                    else value = untrimmed;

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

            return value;
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

        private object ConvertToFieldType(object value)
        {
            return value.ConvertTo(TypeNode.Type);
        }

        private static IEnumerable<byte> ReadNullTerminated(BinaryReader reader, int maxLength)
        {
            var buffer = new MemoryStream();

            byte b;
            while (maxLength-- > 0 && (b = reader.ReadByte()) != 0)
                buffer.WriteByte(b);

            return buffer.ToArray();
        }

        public override string ToString()
        {
            if (Name != null)
                return $"{Name}: {Value}";

            return Value?.ToString() ?? base.ToString();
        }
    }
}