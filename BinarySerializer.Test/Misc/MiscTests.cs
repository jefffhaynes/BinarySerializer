using System;
using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    public class CountTests : TestBase
    {
        [Fact]
        public void ThrowIfMemberImplementsIDictionary()
        {
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new DictionaryMemberClass()));
        }

        [Fact]
        public void ThrowIfImplementsIDictionary()
        {
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new Dictionary<string, string>()));
        }
    }
}
