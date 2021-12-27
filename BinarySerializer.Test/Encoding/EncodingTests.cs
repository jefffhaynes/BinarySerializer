namespace BinarySerialization.Test.Encoding;

[TestClass]
public class EncodingTests : TestBase
{
    public EncodingTests()
    {
        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [TestMethod]
    public void EncodingTest()
    {
        var expected = new EncodingClass { Name = "السلام عليكم" };
        var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name + char.MinValue);
        var actual = Roundtrip(expected, expectedData);

        Assert.AreEqual(expected.Name, actual.Name);
    }

    [TestMethod]
    public void EncodingTest2()
    {
        var expected = new EncodingClass2 { Name = "السلام عليكم" };
        var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name + char.MinValue);
        var actual = Roundtrip(expected, expectedData);

        Assert.AreEqual(expected.Name, actual.Name);
    }

    [TestMethod]
    public void FieldEncodingTest()
    {
        var expected = new FieldEncodingClass { Value = "السلام عليكم", Encoding = "windows-1256" };
        var encodingFieldData = System.Text.Encoding.UTF8.GetBytes(expected.Encoding + char.MinValue);
        var expectedValueData = System.Text.Encoding.GetEncoding(expected.Encoding).GetBytes(expected.Value + char.MinValue);

        var expectedData = encodingFieldData.Concat(expectedValueData).ToArray();

        var actual = Roundtrip(expected, expectedData);

        Assert.AreEqual(expected.Encoding, actual.Encoding);
        Assert.AreEqual(expected.Value, actual.Value);
    }

    [TestMethod]
    public void ConstFieldEncodingTest()
    {
        var expected = new ConstEncodingClass { Value = "السلام عليكم" };
        var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Value + char.MinValue);
        var actual = Roundtrip(expected, expectedData);

        Assert.AreEqual(expected.Value, actual.Value);
    }

    [TestMethod]
    public void NullTerminatedUtf16Test()
    {
        var expected = new EncodingClassUtf16 { Name = "hello" };
        var expectedData = System.Text.Encoding.Unicode.GetBytes(expected.Name + char.MinValue);
        var actual = Roundtrip(expected, expectedData);

        Assert.AreEqual(expected.Name, actual.Name);
    }
}
