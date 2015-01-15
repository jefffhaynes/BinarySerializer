using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.SerializeAs
{
    [TestClass]
    public class SerializeAsTest : TestBase
    {
        [TestMethod]
        public void SerializeIntAsSizedStringTest()
        {
            var expected = new SizedStringClass<int> {Value = 33};
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes(expected.Value.ToString()));

            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
