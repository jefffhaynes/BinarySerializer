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
            var actual = Roundtrip(expected, new byte[] {0x6, 0, 0, 0});
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void ScaleIntTest()
        {
            var expected = new ScaledIntValueClass { Value = 3 };
            var actual = Roundtrip(expected, new byte[] { 0x6, 0, 0, 0 });
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void NegativeScaleTest()
        {
            var expected = new ScaledValueClass { Value = -3 };
            var actual = Roundtrip(expected, new byte[] { 0xFA, 0xFF, 0xFF, 0xFF });
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void BigEndianScaleTest()
        {
            var expected = new ScaledValueClass { Value = 3 };
            var actual = RoundtripBigEndian(expected, new byte[] { 0, 0, 0, 6 });
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
