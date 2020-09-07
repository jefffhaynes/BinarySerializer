using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue27
{
    [TestClass]
    public class Issue27Tests
    {
        [TestMethod]
        public void TestPrematureStreamTermination()
        {
            var serializer = new BinarySerializer {Endianness = BinarySerialization.Endianness.Little};
            var inBytes = new byte[] {0x01, 0x00, 0x00, 0x00, 0x40, 0x34};

            using (var stream = new MemoryStream(inBytes))
            {
                var actualObj = serializer.Deserialize<LoadCarrierData>(stream);
                Assert.IsNull(actualObj);
            }
        }
    }
}