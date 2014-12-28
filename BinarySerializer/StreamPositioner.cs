using System;
using System.IO;

namespace BinarySerialization
{
    internal class StreamPositioner : IDisposable
    {
        private readonly ulong? _position;
        private readonly Stream _stream;

        public StreamPositioner(Stream stream, IntegerBinding fieldOffsetBinding)
        {
            _stream = stream;

            if (fieldOffsetBinding != null)
            {
                if (!stream.CanSeek)
                    throw new InvalidOperationException("FieldOffsetAttribute not supported for non-seekable streams");

                _position = fieldOffsetBinding.Value;
                _stream.Position = (long)fieldOffsetBinding.Value;
            }
        }

        public void Dispose()
        {
            if (_position != null)
                _stream.Position = (long)_position;
        }
    }
}
