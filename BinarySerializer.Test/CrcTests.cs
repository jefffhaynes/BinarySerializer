using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test
{
    [TestClass]
    public class CrcTests
    {
        private static readonly ushort CcittPolynomial = 0x1021;

        [TestMethod]
        public void Crc16Test()
        {
            var crc = new Crc16(CcittPolynomial, 0xffff);
            var messageData = System.Text.Encoding.ASCII.GetBytes("hello world");
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.AreEqual(0xefeb, final);
        }
    }
}
