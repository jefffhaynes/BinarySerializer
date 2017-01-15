using System;
using System.IO;

namespace BinarySerialization.Test.Issues.Issue38
{
    public class TimestampStruct : IBinarySerializable
    {
        /// <summary> Gets or sets the time of the date. </summary>
        /// <value> The time of the date. </value>
        [Ignore]
        public DateTime Value { get; set; }

        /// <summary> Deserializes to the object. </summary>
        /// <param name="stream">               The stream. </param>
        /// <param name="endianness">           The endianness. </param>
        /// <param name="serializationContext"> Context for the serialization. </param>
        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            var data = new byte[8];
            if (stream.Read(data, 0, data.Length) == -1)
            {
                throw new InvalidOperationException("Could not read stream");
            }

            Value = new DateTime(2000, 1, 1);
        }

        /// <summary> Serializes the object to the stream. </summary>
        /// <param name="stream">               The stream. </param>
        /// <param name="endianness">           The endianness. </param>
        /// <param name="serializationContext"> Context for the serialization. </param>
        public void Serialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            if (Value == DateTime.MinValue)
            {
                stream.Write(new byte[8], 0, 8);
                return;
            }

            var yearByte = Convert.ToSByte(Value.Year - 2000);
            stream.WriteByte((byte)yearByte);
            stream.WriteByte((byte)Value.Month);
            stream.WriteByte((byte)Value.Day);
            stream.WriteByte((byte)Value.Hour);
            stream.WriteByte((byte)Value.Minute);
            stream.WriteByte((byte)Value.Second);

            var msData = BitConverter.GetBytes((ushort) Value.Millisecond);
            stream.Write(msData, 0, sizeof(ushort));
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}