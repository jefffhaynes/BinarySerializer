using System.IO;
using Xunit;

namespace BinarySerialization.Test.Value
{
    public class PngTests : TestBase
    {
        [Fact]
        public void DeserializePng()
        {
            using (var stream = new FileStream(Path.Combine("Value","image.png"), FileMode.Open, FileAccess.Read))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                
                RoundtripReverse<Png>(data);
            }
        }
    }
}
