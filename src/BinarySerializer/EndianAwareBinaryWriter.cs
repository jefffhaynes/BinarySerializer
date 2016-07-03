﻿using System.Text;
using System.IO;

namespace BinarySerialization
{
    /// <summary>
    /// An extension of the <see cref="BinaryWriter"/> class that supports big- and little-endian byte ordering.
    /// </summary>
    public class EndianAwareBinaryWriter : BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class based on the specified
        /// stream and using UTF-8 encoding.
        /// </summary>
        /// <param name="output">The output stream.</param>
        public EndianAwareBinaryWriter(Stream output): base(output) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class based on the specified 
        /// stream and character encoding.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public EndianAwareBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class based on the specified 
        /// stream, character encoding, and endianness.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="endianness">The byte ordering to use.</param>
        public EndianAwareBinaryWriter(Stream output, Encoding encoding, Endianness endianness) : base(output, encoding)
        {
            Endianness = endianness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class based on the specified 
        /// stream and endianness using UTF-8 encoding.
        /// </summary>
        /// <param name="output">The input stream.</param>
        /// <param name="endianness">The byte ordering to use.</param>
        public EndianAwareBinaryWriter(Stream output, Endianness endianness) : base(output)
        {
            Endianness = endianness;
        }

        /// <summary>
        /// The byte ordering to use.
        /// </summary>
        public Endianness Endianness { get; set; }

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(short value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes a two-byte unsigned integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(ushort value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes a four-byte signed integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(int value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(uint value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(long value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(ulong value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes a four-byte floating-point value to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(float value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }

        /// <summary>
        /// Writes an eight-byte floating-point value to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(double value)
        {
            var v = Endianness == Endianness.Big ? Bytes.Reverse(value) : value;
            base.Write(v);
        }
    }
}
