using System;

namespace BinarySerialization.Test.Issues.Issue225
{
    public enum ValueBlockType : UInt32
    {
        PlainValue = 1,
        ValueWithDescriptor = 2
    }
}