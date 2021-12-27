namespace BinarySerialization.Test.Issues.Issue125;

public class TestClass
{
    [FieldOrder(1)]
    [FieldBitLength(2)]
    public byte Header { get; set; }
    [FieldOrder(2)]
    public byte[] Body { get; set; } //N bits
}

public class TestClass2
{
    [FieldOrder(1)]
    public byte Header { get; set; }
    [FieldOrder(2)]
    public byte[] Body { get; set; } //N bits
}

public class Issue125Tests : TestBase
{
    //[TestMethod]
    public void TestMethod1()
    {
        var b = new byte[]
        {
                0xE0, 0x00, 0x00
        };

        var c = Deserialize<TestClass>(b);
        Assert.AreEqual(0x80, c.Body[0]);
        Assert.AreEqual(0x00, c.Body[1]);
    }

    [TestMethod]
    public void TestMethod2()
    {
        var b = new byte[]
        {
                0xE0, 0x80, 0x00
        };

        var c = Deserialize<TestClass2>(b);
        Assert.AreEqual(0x80, c.Body[0]);
        Assert.AreEqual(0x00, c.Body[1]);
    }
}
