namespace BinarySerialization.Test.Issues.Issue55
{
    public class TestChunk : Chunk
    {
        [FieldOrder(0)]
        public CustomSerializable[] Customs;
    }
}