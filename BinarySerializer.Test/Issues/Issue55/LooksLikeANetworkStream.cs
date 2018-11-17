using System;
using System.IO;

namespace BinarySerialization.Test.Issues.Issue55
{
    /// <summary>
    /// Wraps a stream to look like a NetworkStream
    /// </summary>
    public class LooksLikeANetworkStream : Stream
    {

        private Stream UnderlyingStream { get; }

        public LooksLikeANetworkStream(Stream source)
        {
            UnderlyingStream = source;
        }

        public override void Flush()
        {
            UnderlyingStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return UnderlyingStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            UnderlyingStream.Write(buffer, offset, count);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => UnderlyingStream.Position;
            set => UnderlyingStream.Position = value;
        }
    }
}
