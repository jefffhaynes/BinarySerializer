using System;
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
            var serializer = new BinarySerializer {Endianness = BinarySerialization.Endianness.Little};
            var inBytes = new byte[]
            {
                0xFE, 0x31, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x02, Convert.ToByte('E'), Convert.ToByte('m'), Convert.ToByte('p'), Convert.ToByte('t'),
                Convert.ToByte('y'), 0x00
            };
            var expected = new Bin3Data
            {
                BinType = 254,
                Ident = "1",
                Occupancy = BinOccupancy.Full,
                OccupancyString = BinOccupancy.Empty
            };
            
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, expected);
            }

            //Assert.AreEqual(inBytes, actualBytes, "Objects are not equal");

            Bin3Data actual;
            using (var stream = new MemoryStream(inBytes))
            {
                actual = serializer.Deserialize<Bin3Data>(stream);
            }

            Assert.AreEqual(expected.BinType, actual.BinType);
            Assert.AreEqual(expected.Ident, actual.Ident);
            Assert.AreEqual(expected.Occupancy, actual.Occupancy);
            Assert.AreEqual(expected.OccupancyString, actual.OccupancyString);
        }
    }
}