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

        public TapStream(Stream source, Stream tap, string name) : base(source, name)
        {
            _tap = tap;
        }

        public override bool CanSeek => false;

        public override long Position
        {
            get => base.Position;
            set => throw new InvalidOperationException(TappingErrorMessage);
        }

        protected override bool IsByteBarrier => true;

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            _tap.Write(buffer, offset, read);
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
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

        protected override void WriteByteAligned(byte[] buffer, int length)
        {
            _tap.Write(buffer, 0, length);
            base.WriteByteAligned(buffer, length);
        }

        protected override async Task WriteByteAlignedAsync(byte[] buffer, int length,
            CancellationToken cancellationToken)
        {
            await _tap.WriteAsync(buffer, 0, length, cancellationToken).ConfigureAwait(false);
            await base.WriteByteAlignedAsync(buffer, length, cancellationToken).ConfigureAwait(false);
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await _tap.FlushAsync(cancellationToken).ConfigureAwait(false);
            await base.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public override void Flush()
        {
            _tap.Flush();
            base.Flush();
        }
    }
}