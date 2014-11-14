using System.IO;

namespace BinarySerialization
{
    /// <summary>
    /// Used to wrap streams to allow for relative position tracking whether the source stream is seekable or not.
    /// </summary>
    internal class StreamKeeper : Stream
    {
        private readonly Stream _stream;

        public long RelativePosition { get; set; }

        public StreamKeeper(Stream stream)
        {
            _stream = stream;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long startingPosition = 0;

            /* If an exception is going to be thrown, we want the stack to look right */
            if (_stream.CanSeek)
                startingPosition = _stream.Position;

            var finalPosition = _stream.Seek(offset, origin);

            RelativePosition += (finalPosition - startingPosition);

            return finalPosition;
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _stream.Read(buffer, offset, count);
            RelativePosition += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
            RelativePosition += count;
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

        public override long Position
        {
            get { return _stream.Position; }
            set { Seek(value, SeekOrigin.Begin); }
        }
    }
}
