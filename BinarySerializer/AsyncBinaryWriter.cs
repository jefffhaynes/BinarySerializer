using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    internal class AsyncBinaryWriter : BinaryWriter
    {
        public AsyncBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public Task WriteAsync(byte value, CancellationToken cancellationToken)
        {
            var data = new[] { value };
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(sbyte value, CancellationToken cancellationToken)
        {
            var data = new[] { (byte)value };
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(short value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(ushort value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(int value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(uint value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(long value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(ulong value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(float value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(double value, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, cancellationToken);
        }

        public Task WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            return BaseStream.WriteAsync(data, 0, data.Length, cancellationToken);
        }

        public Task WriteAsync(byte[] data, int length, CancellationToken cancellationToken)
        {
            return BaseStream.WriteAsync(data, 0, length, cancellationToken);
        }
    }
}
