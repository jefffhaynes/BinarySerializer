using System;
using System.IO;

namespace BinarySerialization
{
    internal class StreamResetter : IDisposable
    {
        private long? _position;
        private readonly Stream _stream;

        public StreamResetter(Stream stream, bool resetOnDispose = true)
        {
            if (!resetOnDispose)
                return;

            _stream = stream;
            _position = _stream.Position;
        }

        public void CancelReset()
        {
            _position = null;
        }

        public void Dispose()
        {
            if (_position == null)
                return;

            if (!_stream.CanSeek)
                throw new InvalidOperationException("Not supported on non-seekable streams");

            _stream.Position = (long)_position;
        }
    }
}
