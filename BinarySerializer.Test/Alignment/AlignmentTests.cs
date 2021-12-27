namespace BinarySerialization.Test.Alignment;

[TestClass]
public class AlignmentTests : TestBase
{
    [TestMethod]
    public void AlignmentTest()
    {
        var actual = RoundtripReverse<AlignmentClass>(new byte[]
        {
                0x2, 0x0, 0x0, 0x0,
                (byte) 'h', (byte) 'i', 0, 0
        });

        Assert.AreEqual(2, actual.Length);
        Assert.AreEqual("hi", actual.Value);
    }

    [TestMethod]
    public void BoundAlignmentTest()
    {
        var actual = RoundtripReverse<BoundAlignmentClass>(new byte[]
        {
                0x2, 0x4, 0x0, 0x0,
                (byte) 'h', (byte) 'i', 0, 0
        });

        Assert.AreEqual(2, actual.Length);
        Assert.AreEqual(4, actual.Alignment);
        Assert.AreEqual("hi", actual.Value);
    }

    [TestMethod]
    public void LeftAlignmentTest()
    {
        var actual = RoundtripReverse<LeftAlignmentClass>(new byte[]
        {
                0x1, 0x0, 0x0, 0x0,
                0x2,
                0x3
        });

        Assert.AreEqual((byte)1, actual.Header);
        Assert.AreEqual((byte)2, actual.Value);
        Assert.AreEqual((byte)3, actual.Trailer);
    }

    [TestMethod]
    public void RightAlignmentTest()
    {
        var actual = RoundtripReverse<RightAlignmentClass>(new byte[]
        {
                0x1,
                0x2, 0x0, 0x0,
                0x3
        });

        Assert.AreEqual((byte)1, actual.Header);
        Assert.AreEqual((byte)2, actual.Value);
        Assert.AreEqual((byte)3, actual.Trailer);
    }

    [TestMethod]
    public void MixedAlignmentTest()
    {
        var actual = RoundtripReverse<MixedAlignmentClass>(new byte[]
        {
                0x1, 0x0, 0x0, 0x0,
                0x2, 0x0,
                0x3
        });

        Assert.AreEqual((byte)1, actual.Header);
        Assert.AreEqual((byte)2, actual.Value);
        Assert.AreEqual((byte)3, actual.Trailer);
    }
}
