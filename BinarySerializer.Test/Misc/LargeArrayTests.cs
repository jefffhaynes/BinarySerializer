using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Misc
{
    [TestClass]
    public class LargeArrayTests
    {
        [TestMethod]
        public void LargeArrayTest()
        {
            var ser = new BinarySerialization.BinarySerializer();
            var data = new byte[65536 * sizeof(int) * 2];

            var des0 = ser.Deserialize<IntArray64K>(data);

            using (var ms = new MemoryStream(data))
            {
                var des = ser.Deserialize<IntArray64K>(ms);
            }
        }
    }
}
