using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class CollectionMiscTests : TestBase
    {
        [Fact]
        public void ListAtRootTest()
        {
            var expected = new List<string> {"1", "2", "3"};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Count, actual.Count);
        }

        [Fact]
        public void ArrayAtRootTest()
        {
            var expected = new[] {"a", "b", "c"};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Length, actual.Length);
        }
    }
}