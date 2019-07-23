using Xunit;

namespace BinarySerialization.Test.Issues.Issue122
{
    public class Issue122Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var foo = new Foo();
            Roundtrip(foo, new byte[] {0x21});
        }
    }
}
