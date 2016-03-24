using System;

namespace BinarySerialization.Test.Events
{
    public class EventTestInnerClass
    {
        public int Value { get; set; }

        [Ignore]
        public TimeSpan IgnoreMe { get; set; }
    }
}
