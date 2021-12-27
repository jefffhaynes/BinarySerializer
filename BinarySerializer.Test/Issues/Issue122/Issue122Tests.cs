namespace BinarySerialization.Test.Issues.Issue122;

[TestClass]
public class Issue122Tests : TestBase
{
    [TestMethod]
    public void Test()
    {
        var foo = new Foo();
        Roundtrip(foo, new byte[] { 0x21 });
    }

    [TestMethod]
    public void LaserFrameTest()
    {
        var expected = new LaserFrame { X = 0x123, Y = 0x456, R = 0x78, G = 0x90, B = 0xAB };
        var actual = Roundtrip(expected, new byte[] { 0x23, 0x61, 0x45, 0x78, 0x90, 0xAB });

        Assert.AreEqual(expected.X, actual.X);
        Assert.AreEqual(expected.Y, actual.Y);
        Assert.AreEqual(expected.Y, actual.Y);
        Assert.AreEqual(expected.Y, actual.Y);
        Assert.AreEqual(expected.B, actual.B);
    }
}
