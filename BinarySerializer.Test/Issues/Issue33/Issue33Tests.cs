using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue33
{
    [TestClass]
    public class Issue33Tests
    {
        [TestMethod]
        public void DeserializeMessage()
        {
            var serializer = new BinarySerializer() { Endianness = BinarySerialization.Endianness.Little };
            var inBytes = new byte[]
                              {
                                  0xFE, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02
                              };
            var expectedObj = new Bin3Data() { BinType = 254, Ident = "1", Occupancy = BinOccupancy.Full };

            Bin3Data actualObj;
            using (var stream = new MemoryStream(inBytes))
            {
                actualObj = serializer.Deserialize<Bin3Data>(stream);
            }

            Assert.AreEqual(expectedObj.BinType, actualObj.BinType);
            Assert.AreEqual(expectedObj.Ident, actualObj.Ident);
            Assert.AreEqual(expectedObj.Occupancy, actualObj.Occupancy);
        }
    }
}
