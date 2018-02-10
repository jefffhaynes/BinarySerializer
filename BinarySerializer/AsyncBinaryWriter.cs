using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    internal class AsyncBinaryWriter : BinaryWriter
    {
        public AsyncBinaryWriter(BoundedStream output, Encoding encoding) : base(output, encoding)
        {
        }

        public Task WriteAsync(byte value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = new[] {value};
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(sbyte value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = new[] {(byte) value};
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(short value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(ushort value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(int value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(uint value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(long value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(ulong value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(float value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(double value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public Task WriteAsync(byte[] data, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            Resize(ref data, fieldLength);

            var length = fieldLength ?? data.Length;
            var boundedStream = (BoundedStream) BaseStream;
            return boundedStream.WriteAsync(data, length, cancellationToken);
        }

        private static void Resize(ref byte[] data, FieldLength length)
        {
            if (length == null)
            {
                return;
            }

            var totalByteCount = (int) length.TotalByteCount;
            if (data.Length != totalByteCount)
            {
                Array.Resize(ref data, totalByteCount);
            }
        }
    }
}
