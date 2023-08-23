using System.Text;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class ValueDescriptor
    {
        [FieldOrder(0)]
        public byte ProductId { get; set; }
        [FieldOrder(1)]
        public byte CategoryId { get; set; }
    }
}
