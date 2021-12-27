namespace BinarySerialization.Test.Issues.Issue24;

[TestClass]
public class Issue24Tests : TestBase
{
    //[TestMethod]
    public void SerializeTest()
    {
        var data = new LoadCarrierData { Data = new Bin1Data() };

        var actual = Roundtrip(data);

        Assert.AreEqual(LoadCarrierType.Bin1, actual.CarrierType);
        Assert.IsTrue(actual.Data is Bin1Data);
    }
}
