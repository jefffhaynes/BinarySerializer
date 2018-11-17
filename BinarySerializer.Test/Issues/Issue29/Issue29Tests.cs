using System;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue29
{
    
    public class Issue29Tests : TestBase
    {
        [Fact]
        public void TestDefaultSerialization()
        {
            var carrierData = new LoadCarrierData(LoadCarrierType.Unknown, null);

#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(carrierData, 2));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(carrierData, 2));
#endif
        }
    }
}