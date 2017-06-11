using System;
using System.IO;

namespace BinarySerialization.Test.Misc
{
    public class UnflushableStream : Stream
    {
        public override void Flush()
        {
            throw new InvalidOperationException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override bool CanRead { get; } = false;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length => throw new InvalidOperationException();
        public override long Position { get; set; }
    }
}
