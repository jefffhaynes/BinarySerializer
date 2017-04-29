using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Scale
{
    [TestClass]
    public class ScaledTests : TestBase
    {
        [TestMethod]
        public void ScaleTest()
        {
            var expected = new ScaledValueClass {Value = 3};
            var actual = Roundtrip(expected, new byte[] {0x6});
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
