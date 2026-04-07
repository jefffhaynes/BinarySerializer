using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    public class AsyncBinaryReader : BinaryReader
    {
        private readonly Encoding _encoding;
        private readonly Endianness _endianness;

        public BoundedStream InputStream { get; }

        public Endianness Endianness { get; }
        
        public AsyncBinaryReader(BoundedStream input, Encoding encoding)
            : this(input, encoding, Endianness.Little)
        {
        }

        public AsyncBinaryReader(BoundedStream input, Encoding encoding, Endianness endianness) : base(input, encoding)
        {
            InputStream = input;
            _encoding = encoding;
            _endianness = endianness;
            Endianness = endianness;
        }

        public override byte ReadByte()
        {
            var b = ReadPrimitive(sizeof(byte), sizeof(byte));
            return b[0];
        }

        public byte ReadByte(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(byte), fieldLength);
            return b[0];
        }

        public async Task<byte> ReadByteAsync(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(byte), sizeof(byte), cancellationToken).ConfigureAwait(false);
            return b[0];
        }

        public async Task<byte> ReadByteAsync(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(byte), fieldLength, cancellationToken).ConfigureAwait(false);
            return b[0];
        }

        public async Task<char> ReadCharAsync(CancellationToken cancellationToken)
        {
            var decoder = _encoding.GetDecoder();

            int read;

            var chars = new char[1];

            do
            {
                var b = await ReadByteAsync(cancellationToken);
                var data = new[] {b};

                read = decoder.GetChars(data, 0, data.Length, chars, 0, false);
            } while (read < chars.Length);

            return chars[0];
        }

        public override sbyte ReadSByte()
        {
            var b = ReadPrimitive(sizeof(sbyte), sizeof(sbyte));
            return (sbyte)b[0];
        }

        public sbyte ReadSByte(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(sbyte), fieldLength);
            return (sbyte)b[0];
        }

        public async Task<sbyte> ReadSByteAsync(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(sbyte), sizeof(sbyte), cancellationToken).ConfigureAwait(false);
            return (sbyte) b[0];
        }

        public async Task<sbyte> ReadSByteAsync(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(sbyte), fieldLength, cancellationToken).ConfigureAwait(false);
            return (sbyte)b[0];
        }

        public override ushort ReadUInt16()
        {
            var b = ReadPrimitive(sizeof(ushort), sizeof(ushort));
            return BitConverter.ToUInt16(b, 0);
        }

        public ushort ReadUInt16(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(ushort), fieldLength);
            return BitConverter.ToUInt16(b, 0);
        }

        public async Task<ushort> ReadUInt16Async(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(ushort), sizeof(ushort), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToUInt16(b, 0);
        }

        public async Task<ushort> ReadUInt16Async(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(ushort), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToUInt16(b, 0);
        }

        public override short ReadInt16()
        {
            var b = ReadPrimitive(sizeof(short), sizeof(short));
            return BitConverter.ToInt16(b, 0);
        }

        public short ReadInt16(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(short), fieldLength);
            return BitConverter.ToInt16(b, 0);
        }

        public async Task<short> ReadInt16Async(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(short), sizeof(short), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToInt16(b, 0);
        }

        public async Task<short> ReadInt16Async(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(short), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToInt16(b, 0);
        }

        public override uint ReadUInt32()
        {
            var b = ReadPrimitive(sizeof(uint), sizeof(uint));
            return BitConverter.ToUInt32(b, 0);
        }

        public uint ReadUInt32(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(uint), fieldLength);
            return BitConverter.ToUInt32(b, 0);
        }

        public async Task<uint> ReadUInt32Async(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(uint), sizeof(uint), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToUInt32(b, 0);
        }

        public async Task<uint> ReadUInt32Async(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(uint), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToUInt32(b, 0);
        }

        public override int ReadInt32()
        {
            var b = ReadPrimitive(sizeof(int), sizeof(int));
            return BitConverter.ToInt32(b, 0);
        }

        public int ReadInt32(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(int), fieldLength);
            return BitConverter.ToInt32(b, 0);
        }

        public async Task<int> ReadInt32Async(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(int), sizeof(int), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToInt32(b, 0);
        }

        public async Task<int> ReadInt32Async(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(int), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToInt32(b, 0);
        }

        public override ulong ReadUInt64()
        {
            var b = ReadPrimitive(sizeof(ulong), sizeof(ulong));
            return BitConverter.ToUInt64(b, 0);
        }

        public ulong ReadUInt64(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(ulong), fieldLength);
            return BitConverter.ToUInt64(b, 0);
        }

        public async Task<ulong> ReadUInt64Async(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(ulong), sizeof(ulong), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToUInt64(b, 0);
        }

        public async Task<ulong> ReadUInt64Async(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(ulong), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToUInt64(b, 0);
        }

        public override long ReadInt64()
        {
            var b = ReadPrimitive(sizeof(long), sizeof(long));
            return BitConverter.ToInt64(b, 0);
        }

        public long ReadInt64(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(long), fieldLength);
            return BitConverter.ToInt64(b, 0);
        }

        public async Task<long> ReadInt64Async(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(long), sizeof(long), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToInt64(b, 0);
        }

        public async Task<long> ReadInt64Async(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(long), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToInt64(b, 0);
        }

        public override float ReadSingle()
        {
            var b = ReadPrimitive(sizeof(float), sizeof(float));
            return BitConverter.ToSingle(b, 0);
        }

        public float ReadSingle(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(float), fieldLength);
            return BitConverter.ToSingle(b, 0);
        }

        public async Task<float> ReadSingleAsync(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(float), sizeof(float), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToSingle(b, 0);
        }

        public async Task<float> ReadSingleAsync(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(float), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToSingle(b, 0);
        }

        public override double ReadDouble()
        {
            var b = ReadPrimitive(sizeof(double), sizeof(double));
            return BitConverter.ToDouble(b, 0);
        }

        public double ReadDouble(FieldLength fieldLength)
        {
            var b = ReadPrimitive(sizeof(double), fieldLength);
            return BitConverter.ToDouble(b, 0);
        }

        public async Task<double> ReadDoubleAsync(CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(double), sizeof(double), cancellationToken).ConfigureAwait(false);
            return BitConverter.ToDouble(b, 0);
        }

        public async Task<double> ReadDoubleAsync(FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var b = await ReadPrimitiveAsync(sizeof(double), fieldLength, cancellationToken).ConfigureAwait(false);
            return BitConverter.ToDouble(b, 0);
        }

        public void Read(byte[] data, FieldLength fieldLength)
        {
            var length = fieldLength ?? data.Length;
            var readLength = InputStream.Read(data, length);

            if (readLength == 0)
            {
                throw new EndOfStreamException();
            }
        }

        public async Task ReadAsync(byte[] data, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var length = fieldLength ?? data.Length;
            var readLength = await InputStream.ReadAsync(data, length, cancellationToken);

            if (readLength == 0)
            {
                throw new EndOfStreamException();
            }
        }

        public async Task<byte[]> ReadBytesAsync(int count, CancellationToken cancellationToken)
        {
            var b = new byte[count];
            await BaseStream.ReadAsync(b, 0, b.Length, cancellationToken)
                .ConfigureAwait(false);
            return b;
        }

        private byte[] ReadPrimitive(int primitiveSize, FieldLength fieldLength)
        {
            var length = fieldLength ?? primitiveSize;
            var serialized = new byte[(int)length.TotalByteCount];
            Read(serialized, length);
            return NormalizePrimitiveBytes(serialized, primitiveSize, length);
        }

        private async Task<byte[]> ReadPrimitiveAsync(int primitiveSize, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var length = fieldLength ?? primitiveSize;
            var serialized = new byte[(int)length.TotalByteCount];
            await ReadAsync(serialized, length, cancellationToken).ConfigureAwait(false);
            return NormalizePrimitiveBytes(serialized, primitiveSize, length);
        }

        private byte[] NormalizePrimitiveBytes(byte[] serialized, int primitiveSize, FieldLength fieldLength)
        {
            if (serialized.Length == primitiveSize && (_endianness != Endianness.Big || fieldLength.BitCount > 0))
            {
                return serialized;
            }

            var normalized = new byte[primitiveSize];
            var copyCount = Math.Min(serialized.Length, primitiveSize);
            var isBigEndianByteAligned = _endianness == Endianness.Big && fieldLength.BitCount == 0;

            if (isBigEndianByteAligned)
            {
                for (var i = 0; i < copyCount; i++)
                {
                    normalized[i] = serialized[serialized.Length - 1 - i];
                }
            }
            else
            {
                Array.Copy(serialized, normalized, copyCount);
            }

            return normalized;
        }
    }
}