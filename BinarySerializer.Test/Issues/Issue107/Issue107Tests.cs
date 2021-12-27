namespace BinarySerialization.Test.Issues.Issue107;

[TestClass]
public class Issue107Tests : TestBase
{
    [TestMethod]
    public void Test()
    {
        var expected = new ClassToSerialize { Field1 = 1, Field2 = 2, Field3 = 3, Field4 = 4, Field5 = 5 };
        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.Field1, actual.Field1);
        Assert.AreEqual(expected.Field2, actual.Field2);
        Assert.AreEqual(expected.Field3, actual.Field3);
        Assert.AreEqual(expected.Field4, actual.Field4);
        Assert.AreEqual(expected.Field5, actual.Field5);
    }
}
