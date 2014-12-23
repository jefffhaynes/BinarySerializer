using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal class ValueNode : Node
    {
        private static readonly Dictionary<Type, Func<object, object>> TypeConverters =
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
            //if (FieldLengthEvaluator != null)
            //{
            //    var source = FieldLengthEvaluator.Source;
            //    if (source != null)
            //    {
            //        source.Bindings.Add(new Binding(() => (object)GetValueLength()));
            //    }
            //}
        }

        private ulong GetValueLength()
        {
            switch (SerializedType)
            {
                case SerializedType.NullTerminatedString:
                    return (ulong) Encoding.GetByteCount((string) Value) + 1;
                case SerializedType.SizedString:
                    return (ulong) Encoding.GetByteCount((string) Value);
                default:
                    throw new NotSupportedException();
            }
        }

        public override object BoundValue
        {
            get
            {
                var value = Value;

                if (Bindings.Any())
                {
                    value = Bindings[0].TargetValueGetter();

                    if (Bindings.Count != 1)
                    {
                        var targetValues = Bindings.Select(binding => binding.TargetValueGetter()).ToArray();

                        if (targetValues.Any(v => !value.Equals(v)))
                            throw new BindingException(
                                "Multiple bindings to a single source must have equivalent target values.");
                    }
                }

                return ConvertToFieldType(value);
            }
        }

        private object ConvertToFieldType(object value)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (type == Type)
                return value;

            /* Special handling for strings */
            if (type == typeof (string) && Type.IsPrimitive)
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

        public override void Serialize(Stream stream)
        {
            if (BoundValue == null)
                return;

            var writer = new EndianAwareBinaryWriter(stream);

            switch (SerializedType)
            {
                case SerializedType.Int1:
                    writer.Write(Convert.ToSByte(BoundValue));
                    break;
                case SerializedType.UInt1:
                    writer.Write(Convert.ToByte(BoundValue));
                    break;
                case SerializedType.Int2:
                    writer.Write(Convert.ToInt16(BoundValue));
                    break;
                case SerializedType.UInt2:
                    writer.Write(Convert.ToUInt16(BoundValue));
                    break;
                case SerializedType.Int4:
                    writer.Write(Convert.ToInt32(BoundValue));
                    break;
                case SerializedType.UInt4:
                    writer.Write(Convert.ToUInt32(BoundValue));
                    break;
                case SerializedType.Int8:
                    writer.Write(Convert.ToInt64(BoundValue));
                    break;
                case SerializedType.UInt8:
                    writer.Write(Convert.ToUInt64(BoundValue));
                    break;
                case SerializedType.Float4:
                    writer.Write(Convert.ToSingle(BoundValue));
                    break;
                case SerializedType.Float8:
                    writer.Write(Convert.ToDouble(BoundValue));
                    break;
                case SerializedType.ByteArray:
                {
                    var data = (byte[]) BoundValue;
                    var length = FieldLengthEvaluator != null
                        ? (int) FieldLengthEvaluator.BoundValue
                        : data.Length;

                    writer.Write(data, 0, length);
                    break;
                }
                case SerializedType.NullTerminatedString:
                {
                    byte[] data = Encoding.GetBytes(BoundValue.ToString());
                    writer.Write(data);
                    writer.Write((byte) 0);
                    break;
                }
                case SerializedType.SizedString:
                {
                    var value = BoundValue.ToString();
                    byte[] data = Encoding.GetBytes(value);

                    if (FieldLengthEvaluator == null)
                        throw new InvalidOperationException("No field length specified on sized string.");

                    if(FieldLengthEvaluator.IsConst)
                        Array.Resize(ref data, (int) FieldLengthEvaluator.BoundValue);

                    writer.Write(data);
                    break;
                }
                case SerializedType.LengthPrefixedString:
                    writer.Write(BoundValue.ToString());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Deserialize(StreamLimiter stream)
        {
            var reader = new EndianAwareBinaryReader(stream);
            object value;
            switch (SerializedType)
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
                        if(FieldLengthEvaluator == null)
                            throw new InvalidOperationException("No length specified on byte array.");

                        // TODO StreamLimiter

                        value = reader.ReadBytes((int)FieldLengthEvaluator.Value);
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
                        if (FieldLengthEvaluator == null)
                            throw new InvalidOperationException("No length specified on sized string.");

                        byte[] data = reader.ReadBytes((int)FieldLengthEvaluator.Value);
                        value = Encoding.GetString(data, 0, data.Length).TrimEnd('\0');
                        break;
                    }
                case SerializedType.LengthPrefixedString:
                    value = reader.ReadString();
                    break;
                default:
                    throw new NotSupportedException();
            }

            Func<object, object> converter;
            Value = TypeConverters.TryGetValue(Type, out converter) ? converter(value) : value;
        }

        private static IEnumerable<byte> ReadNullTerminatedString(BinaryReader reader)
        {
            byte b;
            while ((b = reader.ReadByte()) != 0)
                yield return b;
        }
    }
}
