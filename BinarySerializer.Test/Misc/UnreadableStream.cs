namespace BinarySerialization.Test.Misc;

internal class UnreadableStream : Stream
{
    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => throw new NotSupportedException();

    public override long Length => throw new IOException();

    public override long Position { get; set; }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new IOException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
    }
}
