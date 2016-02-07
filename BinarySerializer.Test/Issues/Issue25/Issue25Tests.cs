using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue25
{
    [TestClass]
    public class Issue25Tests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestStackOverflow()
        {
            var expetedBytes = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            var obj = new LoadCarrierData() { Data = new object() };

            var serializer = new BinarySerializer() { Endianness = BinarySerialization.Endianness.Little };

            byte[] actualBytes = null;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj);
                actualBytes = stream.ToArray();
            }

            if (!expetedBytes.SequenceEqual(actualBytes))
            {
                Assert.Fail(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Bytes are not equal. Expected({2}): {0} Result({3}): {1}",
                        BitConverter.ToString(expetedBytes),
                        BitConverter.ToString(actualBytes),
                        expetedBytes.Length,
                        actualBytes.Length));
            }
        }
    }
}
