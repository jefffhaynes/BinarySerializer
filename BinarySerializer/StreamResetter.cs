namespace BinarySerialization;

internal class StreamResetter : IDisposable
{
    private readonly Stream _stream;
    private long? _position;

    public StreamResetter(Stream stream, bool resetOnDispose = true)
    {
        if (!resetOnDispose)
        {
            return;
        }

        _stream = stream;
        _position = _stream.Position;
    }

    public void Dispose()
    {
        if (_position == null)
        {
            return;
        }

        if (!_stream.CanSeek)
        {
            throw new InvalidOperationException("Not supported on non-seekable streams");
        }

        _stream.Position = (long)_position;
    }

    public void CancelReset()
    {
        _position = null;
    }
}
