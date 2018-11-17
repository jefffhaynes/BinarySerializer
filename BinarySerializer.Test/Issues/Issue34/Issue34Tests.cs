using Xunit;

namespace BinarySerialization.Test.Issues.Issue34
{
    
    public class Issue34Tests : TestBase
    {
        [Fact]
        public void RoundtripString()
        {
            var expected = new S7String {Value = new InternalS7String("hello")};
            var actual = Roundtrip(expected, 7);
        }
    }
}