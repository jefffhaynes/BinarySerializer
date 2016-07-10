using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue24
{
    [TestClass]
    public class Issue24Tests : TestBase
    {
        //[TestMethod]
        public void SerializeTest()
        {
            var data = new LoadCarrierData {Data = new Bin1Data()};

            var actual = Roundtrip(data);

            Assert.AreEqual(actual.CarrierType, LoadCarrierType.Bin1);
            Assert.IsInstanceOfType(actual.Data, typeof (Bin1Data));
        }
    }
}