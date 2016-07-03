using Xunit;

namespace BinarySerialization.Test.Issues.Issue24
{
        public class Issue24Tests : TestBase
    {
        //[TestMethod]
        public void SerializeTest()
        {
            var data = new LoadCarrierData {Data = new Bin1Data()};

            var actual = Roundtrip(data);

            Assert.Equal(actual.CarrierType, LoadCarrierType.Bin1);
            Assert.IsAssignableFrom(typeof(Bin1Data),actual.Data);
        }
    }
}
