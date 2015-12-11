using System.IO;
using BinarySerialization;

namespace BinarySerialization.Test.Streams
{
    public class StreamClass
    {
        public StreamClass()
        {
            TrailingData = "blah blah";
        }

        [FieldOrder(0)]
        public int FieldLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("FieldLength")]
        public Stream Field { get; set; }

        [FieldOrder(2)]
        public string TrailingData { get; set; }
    }
}