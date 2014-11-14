using System.IO;

namespace BinarySerialization
{
    internal class StreamTap : Stream
    {
        private readonly Stream _stream;
        private readonly MemoryStream _tappedStream = new MemoryStream();

        public StreamTap(Stream stream)
        {
            _stream = stream;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _tappedStream.Seek(offset, origin);
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _tappedStream.SetLength(value);
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _stream.Read(buffer, offset, count);
            _tappedStream.Seek(read, SeekOrigin.Current);
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _tappedStream.Write(buffer, offset, count);
            _stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position { get; set; }
    }
}
