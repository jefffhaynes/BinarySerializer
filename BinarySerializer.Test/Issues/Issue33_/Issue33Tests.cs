using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue33
{
    [TestClass]
    public class Issue33Tests
    {
        //[TestMethod]
        public void SerializeMessage()
        {
            var serializer = new BinarySerializer() { Endianness = BinarySerialization.Endianness.Little };
            var expectedBytes = new byte[]
                                    {
                                        0xFE, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x02, Convert.ToByte('E'), Convert.ToByte('m'), Convert.ToByte('p'), Convert.ToByte('t'),
                                        Convert.ToByte('y'), 0x00,
                                    };

            var inObj = new Bin3Data() { BinType = 254, Ident = "1", Occupancy = BinOccupancy.Full, OccupancyString = BinOccupancy.Empty };

            byte[] actualBytes;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, inObj);
                actualBytes = stream.ToArray();
            }

            Assert.AreEqual(expectedBytes, actualBytes, "Objects are not equal");
        }
    }
}
