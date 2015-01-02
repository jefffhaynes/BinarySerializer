using System;
using System.Text;
using System.IO;

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

        public override short ReadInt16()
        {
            var value = base.ReadInt16();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override ushort ReadUInt16()
        {
            var value = base.ReadUInt16();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override int ReadInt32()
        {
            var value = base.ReadInt32();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override uint ReadUInt32()
        {
            var value = base.ReadUInt32();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override long ReadInt64()
        {
            var value = base.ReadInt64();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override ulong ReadUInt64()
        {
            var value = base.ReadUInt64();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override float ReadSingle()
        {
            var value = base.ReadSingle();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override double ReadDouble()
        {
            var value = base.ReadDouble();
            return Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
        }

        public override string ReadString()
        {
            throw new NotSupportedException();
        }
    }
}
