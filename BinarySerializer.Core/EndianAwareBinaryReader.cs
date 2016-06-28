using System.IO;
using System.Text;

namespace BinarySerialization
{
    /// <summary>
    /// An extension of the <see cref="BinaryReader"/> class that supports big- and little-endian byte ordering.
    /// </summary>
    public class EndianAwareBinaryReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryReader"/> class based on the specified
        /// stream and using UTF-8 encoding.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public EndianAwareBinaryReader(Stream input) : base(input) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryReader"/> class based on the specified 
        /// stream and character encoding.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public EndianAwareBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryReader"/> class based on the specified 
        /// stream, character encoding, and endianness.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="endianness">The byte ordering to use.</param>
        public EndianAwareBinaryReader(Stream input, Encoding encoding, Endianness endianness) : base(input, encoding) 
        {
            Endianness = endianness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryReader"/> class based on the specified 
        /// stream and endianness using UTF-8 encoding.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="endianness">The byte ordering to use.</param>
        public EndianAwareBinaryReader(Stream input, Endianness endianness) : base(input)
        {
            Endianness = endianness;
        }

        /// <summary>
        /// The byte ordering to use.
        /// </summary>
        public Endianness Endianness { get; set; }

        /// <summary>
        /// Reads a two-byte signed integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        public override short ReadInt16()
        {
            var value = base.ReadInt16();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads a two-byte unsigned integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        public override ushort ReadUInt16()
        {
            var value = base.ReadUInt16();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads a four-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        public override int ReadInt32()
        {
            var value = base.ReadInt32();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads a four-byte unsigned integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        public override uint ReadUInt32()
        {
            var value = base.ReadUInt32();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads an eight-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        public override long ReadInt64()
        {
            var value = base.ReadInt64();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads an eight-byte unsigned integer from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        public override ulong ReadUInt64()
        {
            var value = base.ReadUInt64();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads a four-byte floating-point value from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        public override float ReadSingle()
        {
            var value = base.ReadSingle();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        /// <summary>
        /// Reads an eight-byte floating-point value from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        public override double ReadDouble()
        {
            var value = base.ReadDouble();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }
    }
}
