namespace BinarySerialization.Test.Issues.Issue61;

[TestClass]
public class Issue61Tests : TestBase
{
    [TestMethod]
    public void ListOfObjectsTest()
    {
        var expected = new List<Message>
            {
                new Message
                {
                    Data = new byte[] {0x0, 0x0, 0x0, 0x0}
                }
            };

        var actual = Roundtrip(expected);
        Assert.AreEqual(1, actual.Count);
    }
}
