using System;
using System.IO;

namespace BinarySerialization.Test.Misc
{
    internal class UnreadableStream : Stream
    {
        public override void Flush()
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new IOException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override long Length
        {
            get { throw new IOException(); }
        }

        public override long Position { get; set; }
    }
}