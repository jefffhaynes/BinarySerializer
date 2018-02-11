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
            OutputStream = output;
        }

        public BoundedStream OutputStream { get; }

        public void Write(byte value, FieldLength fieldLength)
        {
            var data = new[] {value};
            Write(data, fieldLength);
        }

        public Task WriteAsync(byte value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = new[] {value};
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(sbyte value, FieldLength fieldLength)
        {
            var data = new[] {(byte) value};
            Write(data, fieldLength);
        }

        public Task WriteAsync(sbyte value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = new[] {(byte) value};
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(short value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(short value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(ushort value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(ushort value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(int value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(int value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(uint value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(uint value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(long value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(long value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(ulong value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(ulong value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(float value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
        }

        public Task WriteAsync(float value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = BitConverter.GetBytes(value);
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(double value, FieldLength fieldLength)
        {
            var data = BitConverter.GetBytes(value);
            Write(data, fieldLength);
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
            return OutputStream.WriteAsync(data, length, cancellationToken);
        }

        public void Write(byte[] data, FieldLength fieldLength)
        {
            Resize(ref data, fieldLength);

            var length = fieldLength ?? data.Length;
            OutputStream.Write(data, length);
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
