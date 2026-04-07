using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    internal class AsyncBinaryWriter : BinaryWriter
    {
        private readonly Encoding _encoding;
        private readonly Endianness _endianness;
        private readonly byte _paddingValue;

        public AsyncBinaryWriter(BoundedStream output, Encoding encoding, byte paddingValue)
            : this(output, encoding, paddingValue, Endianness.Little)
        {
        }

        public AsyncBinaryWriter(BoundedStream output, Encoding encoding, byte paddingValue, Endianness endianness) : base(output, encoding)
        {
            OutputStream = output;
            _encoding = encoding;
            _endianness = endianness;
            _paddingValue = paddingValue;
        }

        public BoundedStream OutputStream { get; }

        public void Write(byte value, FieldLength fieldLength)
        {
            WritePrimitive(new[] { value }, fieldLength);
        }

        public Task WriteAsync(byte value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(new[] { value }, fieldLength, cancellationToken);
        }

        public Task WriteAsync(char value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var data = _encoding.GetBytes(new[] {value});
            return WriteAsync(data, fieldLength, cancellationToken);
        }

        public void Write(sbyte value, FieldLength fieldLength)
        {
            WritePrimitive(new[] { (byte)value }, fieldLength);
        }

        public Task WriteAsync(sbyte value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(new[] { (byte)value }, fieldLength, cancellationToken);
        }

        public void Write(short value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(short value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(ushort value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(ushort value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(int value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(int value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(uint value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(uint value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(long value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(long value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(ulong value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(ulong value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(float value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(float value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
        }

        public void Write(double value, FieldLength fieldLength)
        {
            WritePrimitive(BitConverter.GetBytes(value), fieldLength);
        }

        public Task WriteAsync(double value, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            return WritePrimitiveAsync(BitConverter.GetBytes(value), fieldLength, cancellationToken);
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

        private void WritePrimitive(byte[] data, FieldLength fieldLength)
        {
            var prepared = PreparePrimitiveData(data, fieldLength);
            var length = fieldLength ?? prepared.Length;
            OutputStream.Write(prepared, length);
        }

        private Task WritePrimitiveAsync(byte[] data, FieldLength fieldLength, CancellationToken cancellationToken)
        {
            var prepared = PreparePrimitiveData(data, fieldLength);
            var length = fieldLength ?? prepared.Length;
            return OutputStream.WriteAsync(prepared, length, cancellationToken);
        }

        private byte[] PreparePrimitiveData(byte[] data, FieldLength fieldLength)
        {
            var totalByteCount = fieldLength != null ? (int)fieldLength.TotalByteCount : data.Length;

            if (totalByteCount == data.Length && (_endianness != Endianness.Big || fieldLength?.BitCount > 0))
            {
                return data;
            }

            var prepared = new byte[totalByteCount];
            var copyCount = Math.Min(data.Length, totalByteCount);
            var isBigEndianByteAligned = _endianness == Endianness.Big && (fieldLength == null || fieldLength.BitCount == 0);

            if (isBigEndianByteAligned)
            {
                for (var i = 0; i < copyCount; i++)
                {
                    prepared[totalByteCount - 1 - i] = data[i];
                }
            }
            else
            {
                Array.Copy(data, prepared, copyCount);
            }

            if (_paddingValue == default || copyCount == totalByteCount)
            {
                return prepared;
            }

            if (isBigEndianByteAligned)
            {
                for (var i = 0; i < totalByteCount - copyCount; i++)
                {
                    prepared[i] = _paddingValue;
                }
            }
            else
            {
                for (var i = copyCount; i < totalByteCount; i++)
                {
                    prepared[i] = _paddingValue;
                }
            }

            return prepared;
        }

        private void Resize(ref byte[] data, FieldLength length)
        {
            if (length == null)
            {
                return;
            }

            var dataLength = data.Length;
            var totalByteCount = (int) length.TotalByteCount;

            if (dataLength == totalByteCount)
            {
                return;
            }

            Array.Resize(ref data, totalByteCount);

            if (_paddingValue == default)
            {
                return;
            }

            for (int i = dataLength; i < totalByteCount; i++)
            {
                data[i] = _paddingValue;
            }
        }
    }
}
