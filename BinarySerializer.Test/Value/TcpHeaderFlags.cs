namespace BinarySerialization.Test.Value
{
    public enum TcpHeaderFlags : byte
    {
        Finished = 0x1,
        Synchronize = 0x2,
        Reset = 0x4,
        Push = 0x8,
        Acknowledge = 0x10,
        Urgent = 0x20,
        EcnEcho = 0x40,
        CongestionWindowReduced = 0x80
    }
}