namespace BinarySerialization.Test.Issues.Issue98
{
    public class AbstractOrder
    {
        [SerializeAs(SerializedType.UInt2)]
        public virtual string RequestId { get; set; }
    }
}
