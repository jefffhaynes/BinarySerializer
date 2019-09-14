namespace BinarySerialization.Test.Issues.Issue139
{
    public class Question : IQuestion
    {
        [FieldOrder(0)]
        [SubtypeDefault(typeof(Domain))]
        public IDomain Domain { get; set; }
    }
}
