namespace BinarySerialization.Test.Misc;

[TestClass]
public class IOExceptionTest
{
    [TestMethod]
    public void ShouldThrowIOExceptionNotInvalidOperationExceptionTest()
    {
        var stream = new UnreadableStream();
        var serializer = new BinarySerializer();
        Assert.ThrowsException<IOException>(() => serializer.Deserialize<int>(stream));
    }
}
