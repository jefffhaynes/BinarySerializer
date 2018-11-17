using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

        public override bool CanSeek => false;

        public override long Position
        {
            get => base.Position;
            set => throw new InvalidOperationException(TappingErrorMessage);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            _tap.Write(buffer, offset, read);
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await base.ReadAsync(buffer, offset, count, cancellationToken);
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

        public override void Flush()
        {
            _tap.Flush();
            base.Flush();
        }
    }
}