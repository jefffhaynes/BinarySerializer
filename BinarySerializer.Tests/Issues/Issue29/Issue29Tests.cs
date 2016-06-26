using System;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue29
{
        public class Issue29Tests : TestBase
    {
        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void TestDefaultSerialization()
        {
            var carrierData = new LoadCarrierData(LoadCarrierType.Unknown, null);

           
            var e = Record.Exception(() =>
           Roundtrip(carrierData,2));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }
    }
}
