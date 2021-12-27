namespace BinarySerialization.Test.Endianness;

[TestClass]
public class EndiannessTests : TestBase
{
    [TestMethod]
    public void SerializerEndiannessTest()
    {
        var serializer = new BinarySerializer { Endianness = BinarySerialization.Endianness.Big };
        var expected = new EndiannessClass { Short = 1 };

        var stream = new MemoryStream();
        serializer.Serialize(stream, expected);

        var data = stream.ToArray();

        Assert.AreEqual(0x1, data[1]);
    }

    [TestMethod]
    public void FieldEndiannessBeTest()
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(EndiannessConverter.BigEndiannessMagic);

        writer.Write((byte)0x0);
        writer.Write((byte)0x1);

        var data = stream.ToArray();

        var actual = RoundtripReverse<FieldEndiannessClass>(data);

        Assert.AreEqual(1, actual.Value);
    }

    [TestMethod]
    public void FieldEndiannessLeTest()
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(EndiannessConverter.LittleEndiannessMagic);

        writer.Write((byte)0x1);
        writer.Write((byte)0x0);

        var data = stream.ToArray();

        var actual = RoundtripReverse<FieldEndiannessClass>(data);

        Assert.AreEqual(1, actual.Value);
    }

    [TestMethod]
    public void FieldEndiannessConstTest()
    {
        var expected = new FieldEndiannessConstClass { Value = 1 };
        var actual = Roundtrip(expected, new byte[] { 0x0, 0x0, 0x0, 0x1 });
        Assert.AreEqual(expected.Value, actual.Value);
    }

    [TestMethod]
    public void DeferredFieldEndiannessBeTest()
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write((byte)0x0);
        writer.Write((byte)0x1);

        writer.Write(EndiannessConverter.BigEndiannessMagic);

        var data = stream.ToArray();

        var actual = RoundtripReverse<DeferredEndiannessEvaluationClass>(data);

        Assert.AreEqual(1, actual.Value);
    }

    //[TestMethod]
    //public void InvalidFieldEndiannessConverterTest()
    //{
    //    Assert.Throws<InvalidOperationException>(() => Roundtrip(typeof(FieldEndiannessInvalidConverterClass)));
    //}
}
