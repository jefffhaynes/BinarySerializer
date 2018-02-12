using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    public class AsyncBinaryReader : BinaryReader
    {
        public BoundedStream InputStream { get; }

        public AsyncBinaryReader(BoundedStream input, Encoding encoding) : base(input, encoding)
        {
            InputStream = input;
        }

        public override byte ReadByte()
        {
            var b = new byte[sizeof(byte)];
            Read(b, b.Length);
            return b[0];
        }

        public async Task<byte> ReadByteAsync(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(byte)];
            await ReadAsync(b, b.Length, cancellationToken);
            return b[0];
        }

        public override sbyte ReadSByte()
        {
            var b = new byte[sizeof(sbyte)];
            Read(b, b.Length);
            return (sbyte)b[0];
        }

        public async Task<sbyte> ReadSByteAsync(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(sbyte)];
            await ReadAsync(b, b.Length, cancellationToken);
            return (sbyte) b[0];
        }

        public override ushort ReadUInt16()
        {
            var b = new byte[sizeof(ushort)];
            Read(b, b.Length);
            return BitConverter.ToUInt16(b, 0);
        }

        public async Task<ushort> ReadUInt16Async(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(ushort)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToUInt16(b, 0);
        }

        public override short ReadInt16()
        {
            var b = new byte[sizeof(short)];
            Read(b, b.Length);
            return BitConverter.ToInt16(b, 0);
        }

        public async Task<short> ReadInt16Async(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(short)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToInt16(b, 0);
        }

        public override uint ReadUInt32()
        {
            var b = new byte[sizeof(uint)];
            Read(b, b.Length);
            return BitConverter.ToUInt32(b, 0);
        }

        public async Task<uint> ReadUInt32Async(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(uint)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToUInt32(b, 0);
        }

        public override int ReadInt32()
        {
            var b = new byte[sizeof(int)];
            Read(b, b.Length);
            return BitConverter.ToInt32(b, 0);
        }

        public async Task<int> ReadInt32Async(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(int)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToInt32(b, 0);
        }

        public override ulong ReadUInt64()
        {
            var b = new byte[sizeof(ulong)];
            Read(b, b.Length);
            return BitConverter.ToUInt64(b, 0);
        }

        public async Task<ulong> ReadUInt64Async(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(ulong)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToUInt64(b, 0);
        }

        public override long ReadInt64()
        {
            var b = new byte[sizeof(long)];
            Read(b, b.Length);
            return BitConverter.ToInt64(b, 0);
        }

        public async Task<long> ReadInt64Async(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(long)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToInt64(b, 0);
        }

        public override float ReadSingle()
        {
            var b = new byte[sizeof(float)];
            Read(b, b.Length);
            return BitConverter.ToSingle(b, 0);
        }

        public async Task<float> ReadSingleAsync(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(float)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToSingle(b, 0);
        }

        public override double ReadDouble()
        {
            var b = new byte[sizeof(double)];
            Read(b, b.Length);
            return BitConverter.ToDouble(b, 0);
        }

        public async Task<double> ReadDoubleAsync(CancellationToken cancellationToken)
        {
            var b = new byte[sizeof(double)];
            await ReadAsync(b, b.Length, cancellationToken);
            return BitConverter.ToDouble(b, 0);
        }

        public FieldLength Read(byte[] data, FieldLength fieldLength)
        {
            var length = fieldLength ?? data.Length;
            return InputStream.Read(data, length);
        }

        public Task<FieldLength> ReadAsync(byte[] data, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var length = fieldLength ?? data.Length;
            return InputStream.ReadAsync(data, length, cancellationToken);
        }

        public async Task<byte[]> ReadBytesAsync(int count, CancellationToken cancellationToken)
        {
            var b = new byte[count];
            await BaseStream.ReadAsync(b, 0, b.Length, cancellationToken);
            return b;
        }
    }
}