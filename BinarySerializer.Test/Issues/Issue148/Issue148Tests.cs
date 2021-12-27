namespace BinarySerialization.Test.Issues.Issue148;

[TestClass]
public class Issue148Tests : TestBase
{
    [TestMethod]
    public void RoundtripTest()
    {
        var expected = new ProtocolClass
        {
            Header = new HeaderClass
            {
                ITEM1 = 1,     // 0b1
                ITEM2 = 0x7f,  // 0b1111111
                ITEM3 = 1,     // 0b1
                ITEM4 = 0x7    // 0b111
            }
        };

        var actual = Roundtrip(expected, new byte[] { 0xff, 0x0f });

        Assert.AreEqual(expected.Header.ITEM1, actual.Header.ITEM1);
        Assert.AreEqual(expected.Header.ITEM2, actual.Header.ITEM2);
        Assert.AreEqual(expected.Header.ITEM3, actual.Header.ITEM3);
        Assert.AreEqual(expected.Header.ITEM4, actual.Header.ITEM4);
        Assert.AreEqual(expected.Body, actual.Body);
    }

    [TestMethod]
    public void ProtocolClassTest()
    {
        var bytes = new byte[] { 0xff, 0x0f };
        BinarySerializer serializer = new();
        var protocol = serializer.Deserialize<ProtocolClass>(bytes);
        Assert.AreEqual(0x01, protocol.Header.ITEM1);
        Assert.AreEqual(0x7f, protocol.Header.ITEM2);
        Assert.AreEqual(0x01, protocol.Header.ITEM3);
        Assert.AreEqual(0x07, protocol.Header.ITEM4);
    }
}
