namespace BinarySerialization.Test.Issues.Issue151
{
    public class UserData : dPayload
    {
        [FieldOrder(0)]
        public byte SA { get; set; }
        [FieldOrder(1)]
        public byte TA { get; set; }
        [FieldOrder(2)]
        public byte[] UD { get; set; } // Size is equal to PayloadLength - 2
    }
}