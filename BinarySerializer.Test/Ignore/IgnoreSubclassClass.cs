using System;

namespace BinarySerialization.Test.Ignore
{
    class IgnoreSubclassClass : IgnoreSubclassBaseClass
    {
        public int B { get; set; }

        [Ignore]
        public TimeSpan IgnoreMe { get; set; }
        
        [Ignore]
        public DateTime IgnoreMe2 { get; set; }
    }
}
