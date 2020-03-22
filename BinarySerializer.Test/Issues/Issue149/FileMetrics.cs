namespace BinarySerialization.Test.Issues.Issue149
{
    public class FileMetrics
    {
        [FieldOrder(1)] public short leader; // 0
        [FieldOrder(2)] public uint fileVersion; //2
        [FieldOrder(3)] public int headerLength; //6
        [FieldOrder(4)] public short numChannels; //10
    }
}
