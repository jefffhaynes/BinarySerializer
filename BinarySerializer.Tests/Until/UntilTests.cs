using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Until
{
        public class UntilTests : TestBase
    {
        [Fact]
        public void TestUntilConst()
        {
            var expected = new UntilTestClass<string> { Items = new List<string> { "unless", "someone", "like", "you" }, AfterItems = "a whole awful lot" };
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Items.Count, actual.Items.Count);
            Assert.Equal(expected.AfterItems, actual.AfterItems);
        }

        [Fact]
        public void PrimitiveTestUntilConst()
        {
            var expected = new UntilTestClass<int> { Items = new List<int> { 3,2,1 }, AfterItems = "a whole awful lot" };
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Items.Count, actual.Items.Count);
            Assert.Equal(expected.AfterItems, actual.AfterItems);
        }
    }
}
