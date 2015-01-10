using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.TypeGraph
{
    internal class ValueNode : Node
    {
        protected static readonly Dictionary<Type, Func<object, object>> TypeConverters =
            new Dictionary<Type, Func<object, object>>
            {
                {typeof (char), o => Convert.ToChar(o)},
                {typeof (byte), o => Convert.ToByte(o)},
                {typeof (sbyte), o => Convert.ToSByte(o)},
                {typeof (bool), o => Convert.ToBoolean(o)},
                {typeof (Int16), o => Convert.ToInt16(o)},
                {typeof (Int32), o => Convert.ToInt32(o)},
                {typeof (Int64), o => Convert.ToInt64(o)},
                {typeof (UInt16), o => Convert.ToUInt16(o)},
                {typeof (UInt32), o => Convert.ToUInt32(o)},
                {typeof (UInt64), o => Convert.ToUInt64(o)},
                {typeof (Single), o => Convert.ToSingle(o)},
                {typeof (Double), o => Convert.ToDouble(o)},
                {typeof (string), Convert.ToString}
            };

        public ValueNode(Node parent, Type type) : base(parent, type)
        {
        }

        public ValueNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        //public override object BoundValue
        //{
        //    get
        //    {
        //        object value;

        //        if (Bindings.Any())
        //        {
        //            value = Bindings[0].GetTargetValue();

        //            if (Bindings.Count != 1)
        //            {
        //                var targetValues = Bindings.Select(binding => binding.GetTargetValue()).ToArray();

        //                if (targetValues.Any(v => !value.Equals(v)))
        //                    throw new BindingException(
        //                        "Multiple bindings to a single source must have equivalent target values.");
        //            }
        //        }
        //        else value = Value;

        //        return ConvertToFieldType(value);
        //    }
        //}

        public override void SerializeOverride(Stream stream, object value)
        {
            Serialize(stream, value, GetSerializedType());
        }

        protected void Serialize(Stream stream, object value, SerializedType serializedType, int? length = null)
        {
            if (value == null)
                return;

            var writer = new EndianAwareBinaryWriter(stream, Endianness);

            Serialize(writer, value, serializedType, length);
        }

        public void Serialize(EndianAwareBinaryWriter writer, object value, SerializedType serializedType, int? length = null)
        {
            if (value == null)
                return;

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
                        var data = (byte[])value;
                        writer.Write(data, 0, data.Length);
                        break;
                    }
                case SerializedType.NullTerminatedString:
                    {
                        byte[] data = Encoding.GetBytes(value.ToString());
                        writer.Write(data);
                        writer.Write((byte)0);
                        break;
                    }
                case SerializedType.SizedString:
                    {
                        byte[] data = Encoding.GetBytes(value.ToString());

                        if (length != null)
                        {
                            Array.Resize(ref data, length.Value);
                        }
                        //else if (FieldLengthBinding != null)
                        //{
                        //    if (FieldLengthBinding.IsConst)
                        //        Array.Resize(ref data, (int)FieldLengthBinding.BoundValue);
                        //}
                        //else if (ItemLengthBinding != null)
                        //{
                        //    if (ItemLengthBinding.IsConst)
                        //        Array.Resize(ref data, (int)ItemLengthBinding.BoundValue);
                        //}
                        //else throw new InvalidOperationException("No field length specified on sized string.");

                        writer.Write(data);

                        break;
                    }
                case SerializedType.LengthPrefixedString:
                    {
                        byte[] data = Encoding.GetBytes(value.ToString());
                        var datalength = (ushort)data.Length;

                        writer.Write(datalength);
                        writer.Write(data);
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override object DeserializeOverride(StreamLimiter stream)
        {
            var value = Deserialize(stream, GetSerializedType());
            return ConvertToFieldType(value);
        }

        protected object Deserialize(StreamLimiter stream, SerializedType serializedType, int? length = null)
        {
            var reader = new EndianAwareBinaryReader(stream, Endianness);

            return Deserialize(reader, serializedType, length);
        }

        public object Deserialize(EndianAwareBinaryReader reader, SerializedType serializedType, int? length = null)
        {
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
                        //int effectiveLength;
                        //if (length != null)
                        //    effectiveLength = length.Value;
                        //else if (FieldLengthBinding != null)
                        //    effectiveLength = (int)FieldLengthBinding.Value;
                        //else if (ItemLengthBinding != null)
                        //    effectiveLength = (int)ItemLengthBinding.Value;
                        //else if (FieldCountBinding != null)
                        //    effectiveLength = (int) FieldCountBinding.Value;
                        //else throw new InvalidOperationException("No length specified on byte array.");

                        value = reader.ReadBytes(length.Value);
                        break;
                    }
                case SerializedType.NullTerminatedString:
                    {
                        byte[] data = ReadNullTerminatedString(reader).ToArray();
                        value = Encoding.GetString(data, 0, data.Length);
                        break;
                    }
                case SerializedType.SizedString:
                    {
                        //int effectiveLength;
                        //if (length != null)
                        //    effectiveLength = length.Value;
                        //else if (FieldLengthBinding != null)
                        //    effectiveLength = (int)FieldLengthBinding.Value;
                        //else if (ItemLengthBinding != null)
                        //    effectiveLength = (int)ItemLengthBinding.Value;
                        //else throw new InvalidOperationException("No length specified on sized string.");

                        byte[] data = reader.ReadBytes(length.Value);
                        value = Encoding.GetString(data, 0, data.Length).TrimEnd('\0');
                        break;
                    }
                case SerializedType.LengthPrefixedString:
                    {
                        var dataLength = reader.ReadUInt16();
                        byte[] data = reader.ReadBytes(dataLength);
                        value = Encoding.GetString(data, 0, data.Length).TrimEnd('\0');
                        break;
                    }

                default:
                    throw new NotSupportedException();
            }

            return value;
        }

        public object ConvertToFieldType(object value)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (type == Type)
                return value;

            /* Special handling for strings */
            if (type == typeof(string) && Type.IsPrimitive)
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    value = 0;
            }

            Func<object, object> converter;
            if (TypeConverters.TryGetValue(Type, out converter))
                return converter(value);

            if (Type.IsEnum && value.GetType().IsPrimitive)
                return Enum.ToObject(Type, value);

            return value;
        }

        private static IEnumerable<byte> ReadNullTerminatedString(BinaryReader reader)
        {
            var buffer = new MemoryStream();

            byte b;
            while ((b = reader.ReadByte()) != 0)
                buffer.WriteByte(b);

            return buffer.ToArray();
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}