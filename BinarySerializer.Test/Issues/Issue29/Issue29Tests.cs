namespace BinarySerialization.Test.Issues.Issue29;

[TestClass]
public class Issue29Tests : TestBase
{
    [TestMethod]
    public void TestDefaultSerialization()
    {
        var carrierData = new LoadCarrierData(LoadCarrierType.Unknown, null);

#if TESTASYNC
        Assert.ThrowsException<AggregateException>(() => Roundtrip(carrierData, 2));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(carrierData, 2));
#endif
    }
}
