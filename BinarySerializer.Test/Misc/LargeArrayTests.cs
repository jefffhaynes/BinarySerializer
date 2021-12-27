namespace BinarySerialization.Test.Misc;

[TestClass]
public class LargeArrayTests
{
    [TestMethod]
    public void LargeArrayTest()
    {
        var ser = new BinarySerializer();
        var data = new byte[65536 * sizeof(int) * 2];

        ser.Deserialize<IntArray64K>(data);

        using (var ms = new MemoryStream(data))
        {
            ser.Deserialize<IntArray64K>(ms);
        }
    }
}
