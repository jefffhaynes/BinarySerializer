namespace BinarySerialization.Test.Issues.Issue80;

[TestClass]
public class Issue80Tests : TestBase
{
    [TestMethod]
    public void Test0Xff()
    {
        var expected = new CustomField();
        var actual = Roundtrip(expected, new byte[] { 0xff });
        Assert.IsNull(actual.Value);
    }

    [TestMethod]
    public void TestNormal()
    {
        var expected = new CustomField { Value = "hi" };
        var actual = Roundtrip(expected);
        Assert.AreEqual(expected.Value, actual.Value);
    }
}
