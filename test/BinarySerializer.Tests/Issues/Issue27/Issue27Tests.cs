using System;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue27
{

    //TODO: I'm not sure how to wire this up with xunit...

        public class Issue27Tests
    {
        [Fact]
       // [ExpectedException(typeof(EndOfStreamException))]
        public void TestPrematureStreamTermination()
        {
            var serializer = new BinarySerializer { Endianness = BinarySerialization.Endianness.Little };
            var inBytes = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x40, 0x34 };
            //Exception e;
            //LoadCarrierData actualObj;

            using (var stream = new MemoryStream(inBytes))
            {
                var e = Record.Exception(() => serializer.Deserialize<LoadCarrierData>(stream));
                Assert.NotNull(e);
                Assert.IsType<EndOfStreamException>(e);

                //Assert.Equal(null, actualObj); //, "Deserialization done with invalid Stream.");
      

            }

      
        }
    }
}
