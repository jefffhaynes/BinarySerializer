using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue39
{
    [TestClass]
    public class Issue39Tests : TestBase
    {
        [TestMethod]
        public void DeserializeOnesAndZeros()
        {
            byte[] data = {0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1};
            var inputsStateFrameData = Deserialize<InputsStateFrameData>(data);
            Assert.AreEqual(false, inputsStateFrameData.Inputs[0]);
            Assert.AreEqual(true, inputsStateFrameData.Inputs[1]);
        }
    }
}