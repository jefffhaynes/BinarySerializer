using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerialization
{
    public class AsyncBinaryReader : BinaryReader
    {
        public AsyncBinaryReader(Stream input) : base(input)
        {
        }

        public AsyncBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public AsyncBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public async Task<byte> ReadByteAsync()
        {
            var b = new byte[sizeof(byte)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return b[0];
        }

        public async Task<sbyte> ReadSByteAsync()
        {
            var b = new byte[sizeof(sbyte)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return (sbyte)b[0];
        }

        public async Task<ushort> ReadUInt16Async()
        {
            var b = new byte[sizeof(ushort)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToUInt16(b, 0);
        }

        public async Task<short> ReadInt16Async()
        {
            var b = new byte[sizeof(short)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToInt16(b, 0);
        }

        public async Task<uint> ReadUInt32Async()
        {
            var b = new byte[sizeof(uint)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToUInt32(b, 0);
        }

        public async Task<int> ReadInt32Async()
        {
            var b = new byte[sizeof(int)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToInt32(b, 0);
        }

        public async Task<ulong> ReadUInt64Async()
        {
            var b = new byte[sizeof(ulong)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToUInt64(b, 0);
        }

        public async Task<long> ReadInt64Async()
        {
            var b = new byte[sizeof(long)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToInt64(b, 0);
        }

        public async Task<float> ReadSingleAsync()
        {
            var b = new byte[sizeof(float)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToSingle(b, 0);
        }

        public async Task<double> ReadDoubleAsync()
        {
            var b = new byte[sizeof(double)];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return BitConverter.ToDouble(b, 0);
        }

        public async Task<byte[]> ReadBytesAsync(int count)
        {
            var b = new byte[count];
            await BaseStream.ReadAsync(b, 0, b.Length);
            return b;
        }
    }
}
