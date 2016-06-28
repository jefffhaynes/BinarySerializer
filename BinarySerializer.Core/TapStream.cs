using System;
using System.IO;

namespace BinarySerialization
{
    internal class TapStream : BoundedStream
    {
        private const string TappingErrorMessage = "Not supported while tapping.";
        
        private readonly Stream _tap;

        public TapStream(Stream source, Stream tap) : base(source)
        {
            _tap = tap;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            _tap.Write(buffer, offset, read);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException(TappingErrorMessage);
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException(TappingErrorMessage);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _tap.Write(buffer, offset, count);
            base.Write(buffer, offset, count);
        }
        
        public override bool CanSeek => false;

        public override long Position
        {
            get { return base.Position; }

            set
            {
                throw new InvalidOperationException(TappingErrorMessage);
            }
        }

        public override void Flush()
        {
            _tap.Flush();
            base.Flush();
        }
    }
}
