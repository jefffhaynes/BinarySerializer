using System;
using Xunit;

namespace BinarySerialization.Test.Misc
{
        public class MismatchBindingTests : TestBase
    {
        [Fact]
        public void MismatchBindingTest()
        {
            var expected = new MismatchBindingClass {Name1 = "Alice", Name2 = "Bob"};
            var e = Record.Exception(() => Roundtrip(expected));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);

        }
    }
}
