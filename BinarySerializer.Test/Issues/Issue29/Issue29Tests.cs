using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue29
{
    [TestClass]
    public class Issue29Tests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestDefaultSerialization()
        {
            var carrierData = new LoadCarrierData(LoadCarrierType.Unknown, null);

            Roundtrip(carrierData, 2);
        }
    }
}