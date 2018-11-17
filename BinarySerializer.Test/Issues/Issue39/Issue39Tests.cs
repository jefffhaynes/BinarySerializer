using Xunit;

namespace BinarySerialization.Test.Issues.Issue39
{
    
    public class Issue39Tests : TestBase
    {
        [Fact]
        public void DeserializeOnesAndZeros()
        {
            byte[] data = {0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1, 0x0, 0x1};
            var inputsStateFrameData = Deserialize<InputsStateFrameData>(data);
            Assert.Equal(false, inputsStateFrameData.Inputs[0]);
            Assert.Equal(true, inputsStateFrameData.Inputs[1]);
        }
    }
}