using System;

namespace BinarySerialization.Test.Issues.Issue21
{
    public class FailingClass
    {
        [FieldOrder(0)]
        public byte EncodedDataType { get; set; }

        [Ignore]
        public Type DataType => typeof (bool);
    }
}