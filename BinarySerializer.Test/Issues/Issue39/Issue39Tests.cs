namespace BinarySerialization.Test.Issues.Issue39;

[TestClass]
public class Issue39Tests : TestBase
{
    [TestMethod]
    public void DeserializeOnesAndZeros()
    {
        byte[] data = { 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1 };
        var inputsStateFrameData = Deserialize<InputsStateFrameData>(data);
        Assert.IsFalse(inputsStateFrameData.Inputs[0]);
        Assert.IsTrue(inputsStateFrameData.Inputs[1]);
    }
}
