using BinarySerialization.Test.Issues.Issue139;

namespace BinarySerialization.Test.Issues.Issue140
{
    public class Question : IQuestion
    {
        [FieldOrder(0)]
        [SubtypeDefault(typeof(Domain))] //Check Issue #139 
        public IDomain Domain { get; set; }
    }
}