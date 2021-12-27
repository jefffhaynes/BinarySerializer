namespace BinarySerialization.Test.Misc;

public struct Sample
{
    [FieldOrder(0)]
    public float A;

    [FieldOrder(1)]
    public float B;
}

[TestClass]
public class StructTests : TestBase
{
    [TestMethod]
    public void TestStruct()
    {
        var expected = new Sample();

#if TESTASYNC
        Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<NotSupportedException>(() => Roundtrip(expected));
#endif
    }
}
