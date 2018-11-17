namespace BinarySerialization.Test.Value
{
    public enum TcpOptionKind : byte
    {
        End = 0,
        NoOp = 1,
        MaxSegmentSize = 2,
        WindowScale = 3,
        SackPermitted = 4,
        Sack = 5,
        Echo = 6,
        EchoReply = 7,
        Timestamps = 8,
        PartialOrderConnectionPermitted = 9,
        PartialOrderServiceProfile = 10,
        Cc = 11,
        CcNew = 12,
        CcEcho = 13,
        TcpAlternateChecksumRequest = 14,
        TcpAlternateChecksumData = 15,
        Skeeter = 16,
        Bubba = 17,
        TrailerChecksumOption = 18,
        Md5ChecksumOption = 19,
        ScpsCapabilities = 20,
        SelectiveNegativeAcknowledgements = 21,
        RecordBoundaries = 22,
        CorruptionExperienced = 23,
        Snap = 24,
        TcpCompressionFilter = 26,
        QuickStartResponse = 27,
        UserTimeoutOption = 28,
        TcpAuthenticationOption = 29,
        Multipath = 30,
        TcpFastOpenCookie = 34
    }
}
