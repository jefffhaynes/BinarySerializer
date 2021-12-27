namespace BinarySerialization.Test.Context;

[TestClass]
public class ContextTests
{
    [TestMethod]
    public void ContextTest()
    {
        var contextClass = new ContextClass();
        var serializer = new BinarySerializer();

        var context = new Context { SerializeCondtion = false };

        var stream = new MemoryStream();
        serializer.Serialize(stream, contextClass, context);

        context = new Context { SerializeCondtion = true };

        stream = new MemoryStream();
        serializer.Serialize(stream, contextClass, context);

        Assert.AreEqual(sizeof(int), stream.Length);
    }
}
