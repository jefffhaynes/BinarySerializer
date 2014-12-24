using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerialization
{
    internal class StreamPositioner : IDisposable
    {
        private readonly ulong? _position;
        private readonly Stream _stream;

        public StreamPositioner(Stream stream, IntegerBinding fieldOffsetEvaluator)
        {
            _stream = stream;
            if (fieldOffsetEvaluator != null)
            {
                if (!stream.CanSeek)
                    throw new InvalidOperationException("FieldOffsetAttribute not supported for non-seekable streams");

                _position = fieldOffsetEvaluator.Value;
                _stream.Position = (long)fieldOffsetEvaluator.Value;
            }

        }

        public void Dispose()
        {
            if (_position != null)
                _stream.Position = (long)_position;
        }
    }
}
