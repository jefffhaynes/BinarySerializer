using System;

namespace BinarySerialization.Test.Ignore
{
    public class IgnoreDerivedObjectClass : IgnoreObjectClass
    {
        public int DontIgnoreMe { get; set; }

        [Ignore]
        public TimeSpan IgnoreMe2 { get; set; }
    }
}
