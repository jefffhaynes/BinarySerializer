namespace BinarySerialization.Test.Value;

[TestClass]
public class PngTests : TestBase
{
    [TestMethod]
    public void DeserializePng()
    {
        using var stream = new FileStream("Value\\image.png", FileMode.Open, FileAccess.Read);
        var data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);

        RoundtripReverse<Png>(data);
    }
}
