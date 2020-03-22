using Xunit;

namespace BinarySerialization.Test.Issues.Issue148
{
    public class Issue148Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var parent = new Parent
            {
                Class1 = new Class1 {A = 1},
                Class2 = new Class2 {B = 2}
            };

            Roundtrip(parent, new byte[] {0x11});
        }
    }
}