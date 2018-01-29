namespace BinarySerialization.Test.Issues.Issue98
{
    public class Order : AbstractOrder
    {
        [FieldOrder(0)]
        public int I { get; set; }

        [FieldOrder(1)]
        public int RId { get; set; }

        [Ignore]
        public override string RequestId => RId.ToString("X8");
    }
}