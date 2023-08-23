using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Value
{
    [TestClass]
    public class PngTests : TestBase
    {
        [TestMethod]
        public void DeserializePng()
        {
            byte[] data;

            using (var stream = new FileStream("Value\\image.png", FileMode.Open, FileAccess.Read))
            {
                data = new byte[stream.Length];
                var read = stream.Read(data, 0, data.Length);

                Assert.AreEqual(read, stream.Length);
            }

            RoundtripReverse<Png>(data);
        }
    }
}
