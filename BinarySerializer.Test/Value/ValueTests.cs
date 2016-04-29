using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Value
{
    [TestClass]
    public class ValueTests : TestBase
    {
        [TestMethod]
        public void TestCrc16()
        {
            var expected = new ValueClass
            {
                Internal = new ValueInternalClass
                {
                    Value = "hello world"
                }
            };

            var actual = Roundtrip(expected);
            Assert.AreEqual(0xefeb, actual.Crc);
        }
    }
}
