using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue25
{
    public class Issue25Tests
    {
        [Fact]
        public void TestStackOverflow()
        {
            var expetedBytes = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            var obj = new LoadCarrierData() { Data = new object() };

            var serializer = new BinarySerializer() { Endianness = BinarySerialization.Endianness.Little };

            byte[] actualBytes = null;
            Exception e;

            using (var stream = new MemoryStream())
            {
                e = Record.Exception(() => serializer.Serialize(stream, obj));
                
                actualBytes = stream.ToArray();
            
            }
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);


            //TODO: check if this is okay to remove since the method
            //was already wrapped with ExpectedExceptions attribute

            //if (!expetedBytes.SequenceEqual(actualBytes))
            //{
            //    Assert.True(false,
            //        string.Format(
            //            CultureInfo.InvariantCulture,
            //            "Bytes are not equal. Expected({2}): {0} Result({3}): {1}!!!!!",
            //            BitConverter.ToString(expetedBytes),
            //            BitConverter.ToString(actualBytes),
            //            expetedBytes.Length,
            //            actualBytes.Length));
            //}

            

        }
    }
}
