namespace BinarySerialization;

internal sealed class Crc32 : Crc<uint>
{
    public Crc32(uint polynomial, uint initialValue) : base(polynomial, initialValue)
    {
        IsDataReflected = true;
        IsRemainderReflected = true;
        FinalXor = 0xFFFFFFFF;
    }

    protected override int Width => 32;

    protected override uint ToUInt32(uint value)
    {
        return value;
    }

    protected override uint FromUInt32(uint value)
    {
        return value;
    }
}
