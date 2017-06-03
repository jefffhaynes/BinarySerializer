using System;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    public class BindingTests : TestBase
    {
        [Fact]
        public void InvalidForwardBindingTest()
        {
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(new InvalidForwardBindingClass()));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new InvalidForwardBindingClass());
#endif
        }
    }
}