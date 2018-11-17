using Xunit;

namespace BinarySerialization.Test.Issues.Issue65
{
    
    public class Issue65Tests : TestBase
    {
        [Fact]
        public void CountTest()
        {
            Roundtrip(new TestClass(), 10);
        }

        [Fact]
        public void ComplexLengthTest()
        {
            Roundtrip(new ComplexTestClass {ComplexClass = new ComplexClass()}, 5);
        }
    }
}
