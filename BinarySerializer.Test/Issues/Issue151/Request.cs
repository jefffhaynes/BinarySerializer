namespace BinarySerialization.Test.Issues.Issue151
{
    public class Request : dPayload
    {
        [FieldOrder(0)]
        public byte[] EID { get; set; } // Size is equal to PayloadLength 
    }
}