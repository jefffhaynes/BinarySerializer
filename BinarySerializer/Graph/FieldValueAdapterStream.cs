using System;
using System.IO;

namespace BinarySerialization.Graph
{
    internal class FieldValueAdapterStream : Stream
    {
        private readonly FieldValueAttributeBase _attribute;
        private readonly byte[] _block;
        private int _blockOffset;

        public FieldValueAdapterStream(FieldValueAttributeBase attribute)
        {
            _attribute = attribute;
            _block = new byte[attribute.BlockSize];
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new InvalidOperationException();

        public override long Position
        {
            get => throw new InvalidOperationException();
            set => throw new InvalidOperationException();
        }

        public override void Flush()
        {
            if (_blockOffset > 0)
            {
                _attribute.ComputeInternal(_block, 0, _blockOffset);
            }

            _blockOffset = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "< 0");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "< 0");
            }

            // avoid possible integer overflow
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("array.Length - offset < count");
            }


            // reordered to avoid possible integer overflow
            if (_blockOffset >= _block.Length - count)
            {
                Flush();
                _attribute.ComputeInternal(buffer, offset, count);
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, _block, _blockOffset, count);
                _blockOffset += count;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Flush();
        }
    }
}