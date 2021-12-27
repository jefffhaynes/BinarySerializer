namespace BinarySerialization.Test.WhenNot;

[TestClass]
public class WhenNotTests : TestBase
{
    [TestMethod]
    public void SimpleTest()
    {
        var expected = new WhenNotClass
        {
            ExcludeValue = true,
            Value = 100
        };

        var data = Serialize(expected);
        Assert.AreEqual(1, data.Length);

        expected.ExcludeValue = false;
        data = Serialize(expected);
        Assert.AreEqual(5, data.Length);
    }
}
