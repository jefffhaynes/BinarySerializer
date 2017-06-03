using System.IO;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class LargeArrayTests
    {
        [Fact]
        public void LargeArrayTest()
        {
            var ser = new BinarySerializer();
            var data = new byte[65536*sizeof (int)*2];

            ser.Deserialize<IntArray64K>(data);

            using (var ms = new MemoryStream(data))
            {
                ser.Deserialize<IntArray64K>(ms);
            }
        }
    }
}