using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.FieldBitOrder
{
    [TestClass]
    public class BitOrderTests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var expected = new BitOrderClass
            {
                Value1 = 0x7,
                Value2 = 0x2,
            };

            var actual = Roundtrip(expected, new byte[] { 0x72 } );
            Assert.AreEqual(expected.Value1, actual.Value1);
            Assert.AreEqual(expected.Value2, actual.Value2);
        }

        [TestMethod]
        public void TestForward()
        {
            var expected = new CipMessageRouterDataForward
            {
                Service = CipServiceCodes.GetAttributeSingle,
                Response = false,
            };

            var actual = Roundtrip(expected, new byte[] { 0x0E });
            Assert.AreEqual(expected.Service, actual.Service);
            Assert.AreEqual(expected.Response, actual.Response);
        }

        [TestMethod]
        public void TestBackward()
        {
            var expected = new CipMessageRouterDataBackward
            {
                Service = CipServiceCodes.GetAttributeSingle,
                Response = false,
            };

            var actual = Roundtrip(expected, new byte[] { 0x0E });
            Assert.AreEqual(expected.Service, actual.Service);
            Assert.AreEqual(expected.Response, actual.Response);
        }
    }
}
