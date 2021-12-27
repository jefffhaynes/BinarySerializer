namespace BinarySerialization.Test.Unknown;

[TestClass]
public class UnknownTypeTests : TestBase
{
    [TestMethod]
    public void UnknownTypeSerializationTest()
    {
        var unknownTypeClass = new UnknownTypeClass { Field = "hello" };

        var serializer = new BinarySerializer();

        var stream = new MemoryStream();
        serializer.Serialize(stream, unknownTypeClass);
    }

    [TestMethod]
    public void SubtypesOnUnknownTypeFieldShouldThrowBindingException()
    {
        var unknownTypeClass = new InvalidUnknownTypeClass { Field = "hello" };

        var serializer = new BinarySerializer();

        var stream = new MemoryStream();
        Assert.ThrowsException<BindingException>(() => serializer.Serialize(stream, unknownTypeClass));
    }

    [TestMethod]
    public void BindingAcrossUnknownBoundaryTest()
    {
        var childClass = new BindingAcrossUnknownBoundaryChildClass { Subfield = "hello" };
        var unknownTypeClass = new BindingAcrossUnknownBoundaryClass
        {
            Field = childClass
        };

        var serializer = new BinarySerializer();

        var stream = new MemoryStream();
        serializer.Serialize(stream, unknownTypeClass);

        var data = stream.ToArray();

        Assert.AreEqual((byte)childClass.Subfield.Length, data[0]);
    }

    [TestMethod]
    public void UnknownSubtypeTest()
    {
        var s = "hello";

        var expected = new UnknownSubtypeContainer
        {
            Unknown = new ClassB
            {
                Value = s
            }
        };

        var actual = Roundtrip(expected);

        Assert.IsInstanceOfType(actual.Unknown, typeof(ClassB));
        var value = (ClassB)actual.Unknown;
        Assert.AreEqual(s, value.Value);
    }
}
